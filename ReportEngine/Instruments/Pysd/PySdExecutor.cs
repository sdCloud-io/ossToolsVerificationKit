using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportEngine.FileSystem;
using ReportEngine.InstrumentHelper.Process;
using ReportEngine.models;

namespace ReportEngine.Instruments.Pysd
{
    public class PySdExecutor : IInstrumentExecutor
    {
        private readonly ILogger<PySdExecutor> _logger;
        private readonly IFileSystemHelper _fileSystemHelper;
        private readonly PySdFileValueReader _pySdFileValueReader;
        private string _pySdCmd;
        private readonly Configuration.Configuration _configuration;

        public PySdExecutor(ILogger<PySdExecutor> logger, IFileSystemHelper fileSystemHelper,
            PySdFileValueReader pySdFileValueReader, IOptions<Configuration.Configuration> configuration)
        {
            _logger = logger;
            _fileSystemHelper = fileSystemHelper;
            _pySdFileValueReader = pySdFileValueReader;
            _configuration = configuration.Value;

            Init();
        }

        private void Init()
        {
            _logger.LogTrace(_fileSystemHelper.GetCurrentDirectory());
            var path = _configuration.Instruments.FirstOrDefault(elem => elem.Name == Name)?.Path;
            _logger.LogTrace($"Preparing {Name} for tests");
            _fileSystemHelper.CreateSymbolicLinkDirectory($"./{path}/pysd", "./pysd");
            _pySdCmd = "PySDHelper.py";
            _fileSystemHelper.CopyFile("../PySDHelper.py", $"./{_pySdCmd}");
            _fileSystemHelper.SetPermissionExecute(_pySdCmd);
        }

        public string Name => "PySD";

        public ModelInstrumentResult ExecuteModel(string modelPath)
        {
            var timer = new Stopwatch();
            timer.Start();

            var resultInfo = new ModelInstrumentResult();
            _logger.LogTrace($"Running model with {Name}");
            _logger.LogTrace($"Model path: {modelPath}");

            _logger.LogTrace("Generating model code...");

            _logger.LogTrace("Compiling model code");
            var result = ExecuteCommand("compile", resultInfo, out long compileExecutionTime, modelPath);
            resultInfo.CodeCompilationTime = compileExecutionTime;
            if (!result) return resultInfo;

            _logger.LogTrace("Running compiled model");
            result = ExecuteCommand("compileAndRun", resultInfo, out long compileAndRunExecutionTime, modelPath);
            resultInfo.CodeExecutionTime = compileAndRunExecutionTime;
            if (!result) return resultInfo;

            var timeDelta = timer.ElapsedMilliseconds;
            _logger.LogTrace("========================================================");
            _logger.LogTrace($" Total model processing time with PySD was {timeDelta} ms");
            _logger.LogTrace("========================================================");

            resultInfo.ResultDictionary = _pySdFileValueReader.ReadValues(modelPath);
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
            _logger.LogTrace($" - completed in {executionTime}");
            return string.IsNullOrEmpty(executionProcessResult.Error);
        }
    }
}