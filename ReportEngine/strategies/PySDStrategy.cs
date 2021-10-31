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
            _fileSystemWorker.MoveDirectory(path + "/pysd", "./pysd");
            _pySdCmd = "./PySDHelper.py";
            _fileSystemWorker.CopyFile("../PySDHelper.py", _pySdCmd);
            _fileSystemWorker.SetPermissionExecute(_pySdCmd);
        }

        public ResultInfo ValidateModel(string path, string modelPath)
        {
            _logger.LogInformation($"Running model with {GetName()}");
            _logger.LogInformation($"Model paht: {path}");

            var timer = new Stopwatch();
            var modelProcessingStart = timer.ElapsedMilliseconds;

            _logger.LogInformation("Generating model code...");
            _logger.LogInformation("Compiling model code");

            var modelType = modelPath.Contains(".xmile") ? "compileXmile" : "compileMdl";

            var modelGeneratedCodeCompilationStart = timer.ElapsedMilliseconds;

            var info = new ProcessStartInfo(_pySdCmd)
            {
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                Arguments = $"{modelType} {modelPath} ",
                UseShellExecute = true,
            };
            Process.Start(info);
            return new ResultInfo();
        }

        public string GetName()
        {
            return NAME;
        }
    }
}