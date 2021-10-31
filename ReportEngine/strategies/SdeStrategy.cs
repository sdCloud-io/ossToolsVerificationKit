using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;
using ReportEngine.strategies.interfaces;

namespace ReportEngine.strategies
{
    public class SdeStrategy : IInstrumentStrategy
    {
        private const string NAME = "SDE";
        private readonly ILogger<SdeStrategy> _logger;
        private readonly IFileSystemWorker _fileSystemWorker;

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

            var info = new ProcessStartInfo("npm")
            {
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                Arguments = "install",
                UseShellExecute = true,
            };
            Process.Start(info);

            _fileSystemWorker.ChangeDirectory(currentDir);
        }

        public ResultInfo ValidateModel(string path, string modelPath)
        {
            return new ResultInfo();
        }

        public string GetName()
        {
            return NAME;
        }
    }
}