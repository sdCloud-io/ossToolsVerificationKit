using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ReportEngine.filesystem.adapters;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;
using ReportEngine.strategies.interfaces;

namespace ReportEngine.Instruments
{
    public class PySdExecutor : IInstrumentExecutor
    {
        private const string Name = "PySD";
        private readonly ILogger<PySdExecutor> _logger;
        private readonly IFileSystemHelper _fileSystemHelper;
        private readonly PySDFileAdapter _pySdFileAdapter;
        private string _pySdCmd;

        public PySdExecutor(ILogger<PySdExecutor> logger, IFileSystemHelper fileSystemHelper,
            PySDFileAdapter pySdFileAdapter)
        {
            _logger = logger;
            _fileSystemHelper = fileSystemHelper;
            _pySdFileAdapter = pySdFileAdapter;
        }

        public void Init(string path)
        {
            _logger.LogInformation($"Preparing {GetName()} for tests");
            _fileSystemHelper.CreateSymbolicLinkDirectory(path + "/pysd", "./pysd");
            _pySdCmd = "PySDHelper.py";
            _fileSystemHelper.CopyFile("../PySDHelper.py", _pySdCmd);
            _fileSystemHelper.SetPermissionExecute(_pySdCmd);
        }

        public ModelInstrumentResult ValidateModel(string modelPath)
        {
            var timer = new Stopwatch();
            timer.Start();

            var resultInfo = new ModelInstrumentResult();
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

            resultInfo.ResultDictionary = _pySdFileAdapter.ReadValues(modelPath);
            resultInfo.Result = Constants.Success;
            return resultInfo;
        }

        private bool ExecuteCommand(string command, ModelInstrumentResult resultInfo, out long executionTime,
            string modelPath)
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
            return Name;
        }
    }
}