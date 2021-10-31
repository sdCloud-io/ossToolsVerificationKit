using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;
using ReportEngine.network.interfaces;
using ReportEngine.services.interfaces;
using ReportEngine.strategies;
using ReportEngine.strategies.interfaces;

namespace ReportEngine.services
{
    public class ReportBuilder : IReportService
    {
        private const string ConfigPath = "config.json";
        private const string ScriptsPath = "../scripts.json";
        private const string ReportPath = "testReport.json";
        private readonly ILogger<ReportBuilder> _logger;
        private readonly IFileSystemWorker _fileSystemWorker;
        private readonly IGitLoader _gitLoader;
        private readonly IStrategyProvider _strategyProvider;

        public static Configuration Configuration { get; private set; }

        public ReportBuilder(ILogger<ReportBuilder> logger, IFileSystemWorker fileSystemWorker, IGitLoader gitLoader,
            IStrategyProvider strategyProvider)
        {
            _logger = logger;
            _fileSystemWorker = fileSystemWorker;
            _gitLoader = gitLoader;
            _strategyProvider = strategyProvider;
        }

        public void GenerateReport()
        {
            _logger.LogInformation("Script initial variables:");
            Configuration = _fileSystemWorker.ReadFromJsonFile<Configuration>(ConfigPath);

            _logger.LogInformation("Setting up build directory");
            _fileSystemWorker.DeleteDirectory(Configuration.BuildDir);
            _fileSystemWorker.CreateDirectory(Configuration.BuildDir);
            _fileSystemWorker.ChangeDirectory(Configuration.BuildDir);

            CheckOutTestModels();
            CheckOutInstruments();

            var strategies = _strategyProvider.GetAllStrategies();


            var scripts = _fileSystemWorker.ReadFromJsonFile<List<Script>>(ScriptsPath);

            var report = ExecuteScripts(scripts, strategies);
            _fileSystemWorker.WriteJsonInFile(report, ReportPath);
        }

        private Report ExecuteScripts(List<Script> scripts, List<IInstrumentStrategy> strategies)
        {
            var report = new Report();
            foreach (var script in scripts)
            {
                var scriptResult = new ScriptResult();
                var strategy = strategies.FirstOrDefault(strategy => strategy.GetName() == script.InstrumentName);
                scriptResult.ScriptName = strategy.GetName();

                foreach (var modelPath in _fileSystemWorker.GetFilePathsByExtensions(script.ModelPaths,
                    script.ExtModels))
                {
                    var fixedModelPath = modelPath.Replace("\\", "/");
                    scriptResult.Results.Add(strategy.ValidateModel(fixedModelPath, script.PathResult));
                }

                report.ScriptResults.Add(scriptResult);
            }

            return report;
        }

        private void CheckOutInstruments()
        {
            foreach (var instrument in Configuration.Instruments)
            {
                _gitLoader.DownloadRepository(instrument.Name, instrument.RepositoryUrl, instrument.Path);
            }
        }

        private void CheckOutTestModels()
        {
            foreach (var model in Configuration.Models)
            {
                _gitLoader.DownloadRepository(model.Name, model.RepositoryUrl, model.Path);
            }
        }
    }
}