using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportEngine.FileSystem;
using ReportEngine.InstrumentHelper.Process;
using ReportEngine.models;

namespace ReportEngine.Instruments.Sde
{
    public class SdeExecutor : IInstrumentExecutor
    {
        private readonly ILogger<SdeExecutor> _logger;
        private readonly IFileSystemHelper _fileSystemHelper;
        private readonly SdeFileValueReader _sdeFileValueReader;
        private readonly Configuration.Configuration _configuration;
        private string _sdeCmd;

        public SdeExecutor(ILogger<SdeExecutor> logger, IFileSystemHelper fileSystemHelper,
            SdeFileValueReader sdeFileValueReader, IOptions<Configuration.Configuration> configuration)
        {
            _logger = logger;
            _fileSystemHelper = fileSystemHelper;
            _sdeFileValueReader = sdeFileValueReader;
            _configuration = configuration.Value;

            Init();
        }

        public string Name => "SDEverywhere";

        public ModelInstrumentResult ExecuteModel(string modelPath)
        {
            var timer = new Stopwatch();
            timer.Start();

            var resultInfo = new ModelInstrumentResult();
            _logger.LogTrace($"Running model with {Name}");
            _logger.LogTrace($"Model path: {modelPath}");

            var startGeneratingTime = timer.ElapsedMilliseconds;
            _logger.LogTrace("Generating model code");
            var result = ExecuteCommand("generate", resultInfo, out _, modelPath);
            if (!result) return resultInfo;

            _logger.LogTrace("Generating --genc model code");
            result = ExecuteCommand("generate --genc", resultInfo, out _, modelPath);
            resultInfo.CodeGenerationTime = timer.ElapsedMilliseconds - startGeneratingTime;
            if (!result) return resultInfo;

            _logger.LogTrace("Compiling model code");
            result = ExecuteCommand("compile", resultInfo, out long complieExecutionTime, modelPath);
            resultInfo.CodeCompilationTime = complieExecutionTime;
            if (!result) return resultInfo;

            _logger.LogTrace("Running compiled model");
            result = ExecuteCommand("exec", resultInfo, out long compileAndRunExecutionTime, modelPath);
            resultInfo.CodeExecutionTime = compileAndRunExecutionTime;
            if (!result) return resultInfo;

            var timeDelta = timer.ElapsedMilliseconds;
            _logger.LogTrace("========================================================");
            _logger.LogTrace($" Total model processing time with SDEverywhere was {timeDelta} ms");
            _logger.LogTrace("========================================================");

            resultInfo.ResultDictionary = _sdeFileValueReader.ReadValues(modelPath);
            resultInfo.Result = Constants.Success;
            return resultInfo;
        }

        private bool ExecuteCommand(string command, ModelInstrumentResult resultInfo, out long executionTime,
            string modelPath)
        {
            var timer = new Stopwatch();
            timer.Start();
            var startTime = timer.ElapsedMilliseconds;
            var executionArguments = $"{_sdeCmd} {command} \"{modelPath}\"";
            var executionProcessResult = ProcessHelper.ExecuteProcess("node", executionArguments);
            executionTime = timer.ElapsedMilliseconds - startTime;
            resultInfo.Log = executionProcessResult.Error;
            _logger.LogTrace($" - completed in {executionTime}");
            return string.IsNullOrEmpty(executionProcessResult.Error);
        }

        private void Init()
        {
            var path = _configuration.Instruments.FirstOrDefault(elem => elem.Name == Name)?.Path;
            _logger.LogTrace($"Building {Name} with npm:");
            _logger.LogTrace(_fileSystemHelper.GetCurrentDirectory() + $"./{path}");
            
            var sdeInitProcess = new Process
            {
                StartInfo = new ProcessStartInfo($"npm")
                {
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    Arguments = "install",
                    UseShellExecute = true,
                    WorkingDirectory = $"{path}"
                }
            };
            sdeInitProcess.Start();
            sdeInitProcess.WaitForExit();

            _logger.LogTrace($"Finished building {Name} with npm:");
            _sdeCmd = $"./{path}/src/sde.js";
        }
    }
}