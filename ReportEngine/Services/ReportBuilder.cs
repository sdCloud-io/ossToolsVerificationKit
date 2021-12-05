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
        private readonly IFileSystemHelper _fileSystemHelper;
        private readonly IGitLoader _gitLoader;
        private readonly IStrategyProvider _strategyProvider;

        public static Configuration Configuration { get; private set; }

        public ReportBuilder(ILogger<ReportBuilder> logger, IFileSystemHelper fileSystemHelper, IGitLoader gitLoader,
            IStrategyProvider strategyProvider)
        {
            _logger = logger;
            _fileSystemHelper = fileSystemHelper;
            _gitLoader = gitLoader;
            _strategyProvider = strategyProvider;
        }

        public void GenerateReport()
        {
            var timer = new Stopwatch();
            timer.Start();
            var report = new Report();
            _logger.LogInformation("Script initial variables:");
            Configuration = _fileSystemHelper.ReadFromJsonFile<Configuration>(ConfigPath);

            _logger.LogInformation("Setting up build directory");
            _fileSystemHelper.DeleteDirectory(Configuration.BuildDir);
            _fileSystemHelper.CreateDirectory(Configuration.BuildDir);
            _fileSystemHelper.ChangeDirectory(Configuration.BuildDir);

            CheckOutTestModels();
            CheckOutInstruments();

            var strategies = _strategyProvider.GetAllStrategies();

            var scripts = _fileSystemHelper.ReadFromJsonFile<List<Script>>(ScriptsPath);

            ExecuteScripts(scripts, strategies, report);
            report.SummaryReport.TotalExecutionTime = timer.ElapsedMilliseconds;
            _fileSystemHelper.WriteJsonInFile(report, ReportPath);
        }

        private void ExecuteScripts(List<Script> scripts, List<IInstrumentStrategy> strategies, Report report)
        {
            foreach (var script in scripts)
            {
                var scriptResult = new ScriptResult();
                var strategy = strategies.FirstOrDefault(strategy => strategy.GetName() == script.InstrumentName);
                if (strategy == null) continue;
                scriptResult.ScriptName = script.Name;

                foreach (var modelPath in _fileSystemHelper.GetFilePathsByExtensions(script.ModelPaths,
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