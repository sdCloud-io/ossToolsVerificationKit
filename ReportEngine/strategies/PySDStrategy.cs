using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;
using ReportEngine.strategies.interfaces;

namespace ReportEngine.strategies
{
    public class PySDStrategy : IInstrumentStrategy
    {
        private const string NAME = "PySD";
        private readonly ILogger<PySDStrategy> _logger;
        private readonly IFileSystemWorker _fileSystemWorker;
        private string _pySdCmd;

        public PySDStrategy(ILogger<PySDStrategy> logger, IFileSystemWorker fileSystemWorker)
        {
            _logger = logger;
            _fileSystemWorker = fileSystemWorker;
        }

        public void Init(string path)
        {
            _logger.LogInformation($"Preparing {GetName()} for tests");
            _fileSystemWorker.CreateSymbolicLinkDirectory(path + "/pysd", "./pysd");
            _pySdCmd = "PySDHelper.py";
            _fileSystemWorker.CopyFile("../PySDHelper.py", _pySdCmd);
            _fileSystemWorker.SetPermissionExecute(_pySdCmd);
        }

        public ResultInfo ValidateModel(string modelPath, string modelPathResult)
        {
            var timer = new Stopwatch();
            timer.Start();

            var resultInfo = new ResultInfo { ModelPath = modelPath };
            _logger.LogInformation($"Running model with {GetName()}");
            _logger.LogInformation($"Model path: {modelPath}");

            _logger.LogInformation("Generating model code...");

            _logger.LogInformation("Compiling model code");
            var result = ExecuteCommand("compile", resultInfo, out long complieExecutionTime, modelPath);
            resultInfo.CodeCompilationTime = complieExecutionTime;
            if (!result) return resultInfo;

            _logger.LogInformation("Running compiled model");
            result = ExecuteCommand("compileAndRun", resultInfo, out long compileAndRunExecutionTime, modelPath);
            resultInfo.CodeExecutionTime = compileAndRunExecutionTime;
            if (!result) return resultInfo;

            var timeDelta = timer.ElapsedMilliseconds;
            _logger.LogInformation("========================================================");
            _logger.LogInformation($" Total model processing time with PySD was {timeDelta} ms");
            _logger.LogInformation("========================================================");

            resultInfo.Result = Constants.Success;
            return resultInfo;
        }

        private bool ExecuteCommand(string command, ResultInfo resultInfo, out long executionTime, string modelPath)
        {
            var timer = new Stopwatch();
            timer.Start();
            var startTime = timer.ElapsedMilliseconds;
            var modelType = modelPath.Contains(".xmile") ? "Xmile" : "Mdl";
            var executionCommand = command + modelType;
            var executionArguments = $"{_pySdCmd} {executionCommand} {modelPath}";
            var executionProcessResult = ProcessHelper.ExecuteProcess("python3", executionArguments);
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