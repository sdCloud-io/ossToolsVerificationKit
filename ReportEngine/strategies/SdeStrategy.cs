using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;
using ReportEngine.services;
using ReportEngine.strategies.interfaces;

namespace ReportEngine.strategies
{
    public class SdeStrategy : IInstrumentStrategy
    {
        private const string NAME = "SDE";
        private readonly ILogger<SdeStrategy> _logger;
        private readonly IFileSystemWorker _fileSystemWorker;
        private string sdeCmd;

        public SdeStrategy(ILogger<SdeStrategy> logger, IFileSystemWorker fileSystemWorker)
        {
            _logger = logger;
            _fileSystemWorker = fileSystemWorker;
        }

        public void Init(string path)
        {
            _logger.LogInformation($"Building {GetName()} with npm:");
            var currentDir = _fileSystemWorker.GetCurrentDirectory();
            _fileSystemWorker.ChangeDirectory(path);

            var sdeInitProcess = new Process
            {
                StartInfo = new ProcessStartInfo("npm")
                {
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    Arguments = "install",
                    UseShellExecute = true,
                }
            };
            sdeInitProcess.Start();
            sdeInitProcess.WaitForExit();

            _logger.LogInformation($"Finished building {GetName()} with npm:");
            sdeCmd =
                $"./{ReportBuilder.Configuration.Instruments.FirstOrDefault(elem => elem.Name == NAME)?.Path}/src/sde.js";
            _fileSystemWorker.ChangeDirectory(currentDir);
        }

        public ResultInfo ValidateModel(string modelPath, string modelPathResult)
        {
            var timer = new Stopwatch();
            timer.Start();

            var resultInfo = new ResultInfo { ModelPath = modelPath };
            _logger.LogInformation($"Running model with {GetName()}");
            _logger.LogInformation($"Model path: {modelPath}");

            var startGeneratingTime = timer.ElapsedMilliseconds;
            _logger.LogInformation("Generating model code");
            var result = ExecuteCommand("generate", resultInfo, out _, modelPath);
            if (!result) return resultInfo;

            _logger.LogInformation("Generating --genc model code");
            result = ExecuteCommand("generate --genc", resultInfo, out _, modelPath);
            resultInfo.CodeGenerationTime = timer.ElapsedMilliseconds - startGeneratingTime;
            if (!result) return resultInfo;

            _logger.LogInformation("Compiling model code");
            result = ExecuteCommand("compile", resultInfo, out long complieExecutionTime, modelPath);
            resultInfo.CodeCompilationTime = complieExecutionTime;
            if (!result) return resultInfo;

            _logger.LogInformation("Running compiled model");
            result = ExecuteCommand("exec", resultInfo, out long compileAndRunExecutionTime, modelPath);
            resultInfo.CodeExecutionTime = compileAndRunExecutionTime;
            if (!result) return resultInfo;

            var timeDelta = timer.ElapsedMilliseconds;
            _logger.LogInformation("========================================================");
            _logger.LogInformation($" Total model processing time with SDEverywhere was {timeDelta} ms");
            _logger.LogInformation("========================================================");

            resultInfo.Result = Constants.Success;
            return resultInfo;
        }

        private bool ExecuteCommand(string command, ResultInfo resultInfo, out long executionTime, string modelPath)
        {
            var timer = new Stopwatch();
            timer.Start();
            var startTime = timer.ElapsedMilliseconds;
            var executionCommand = command;
            var executionArguments = $"{sdeCmd} {executionCommand} {modelPath}";
            var executionProcessResult = ProcessHelper.ExecuteProcess("node", executionArguments);
            executionTime = timer.ElapsedMilliseconds - startTime;
            resultInfo.Log = executionProcessResult.Error;
            _logger.LogInformation($" - completed in {executionTime}");
            return string.IsNullOrEmpty(executionProcessResult.Error);
        }

        public string GetName()
        {
            return NAME;
        }
    }
}