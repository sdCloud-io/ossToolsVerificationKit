using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;
using ReportEngine.network.interfaces;
using ReportEngine.services.interfaces;
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
            var timer = new Stopwatch();
            timer.Start();
            var report = new Report();
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

            ExecuteScripts(scripts, strategies, report);
            report.SummaryReport.TotalExecutionTime = timer.ElapsedMilliseconds;
            _fileSystemWorker.WriteJsonInFile(report, ReportPath);
        }

        private Report ExecuteScripts(List<Script> scripts, List<IInstrumentStrategy> strategies, Report report)
        {
            foreach (var script in scripts)
            {
                var scriptResult = new ScriptResult();
                var strategy = strategies.FirstOrDefault(strategy => strategy.GetName() == script.InstrumentName);
                if (strategy == null) continue;
                scriptResult.ScriptName = script.Name;

                foreach (var modelPath in _fileSystemWorker.GetFilePathsByExtensions(script.ModelPaths,
                    script.ExtModels))
                {
                    report.SummaryReport.TotalModelsCount++;
                    var validationModelResult = strategy.ValidateModel(modelPath, script.PathResult);
                    scriptResult.Results.Add(validationModelResult);
                    if (validationModelResult.Result == Constants.Success)
                    {
                        report.SummaryReport.SucceededModelsCount++;
                    }
                    else
                    {
                        report.SummaryReport.FailedModelsCount++;
                    }
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