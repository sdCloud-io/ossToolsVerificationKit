using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;
using ReportEngine.services.interfaces;
using ReportEngine.strategies.interfaces;

namespace ReportEngine.services
{
    public class ReportBuilder : IReportService
    {
        private readonly ILogger<ReportBuilder> _logger;
        private readonly IFileSystemHelper _fileSystemHelper;
        private readonly Configuration _configuration;
        private readonly List<IInstrumentStrategy> _providedStrategies;

        public ReportBuilder(ILogger<ReportBuilder> logger, IFileSystemHelper fileSystemHelper,
            IStrategyProvider strategyProvider, IOptions<Configuration> configuration)
        {
            _logger = logger;
            _fileSystemHelper = fileSystemHelper;
            _configuration = configuration.Value;

            _logger.LogInformation("Setting up build directory");
            _fileSystemHelper.ChangeDirectory(_configuration.BuildDir);

            _providedStrategies = strategyProvider.GetAllStrategies();
        }

        public void GenerateReport()
        {
            var timer = new Stopwatch();
            timer.Start();
            var report = new Report();

            ExecuteScripts(_configuration.Scripts, report);
            report.SummaryReport.TotalExecutionTime = timer.ElapsedMilliseconds;
            _fileSystemHelper.WriteJsonInFile(report, _configuration.ReportFilePath);
        }

        private void ExecuteScripts(List<ScriptConfiguration> scripts,
            Report report)
        {
            var fileExtensions = scripts.GroupBy(script => script.ExtModels).Select(group => new
            {
                FileExtension = group.Key,
                ScriptNames = group.Select(script => new
                {
                    script.Name,
                    script.InstrumentName
                })
            });

            foreach (var fileExtension in fileExtensions)
            {
                foreach (var modelPath in _fileSystemHelper.GetFilePathsByExtensions(
                             fileExtension.FileExtension))
                {
                    var modelResult = new ModelResult
                    {
                        ModelPath = modelPath,
                    };

                    foreach (var scriptName in fileExtension.ScriptNames)
                    {
                        modelResult.ModelInstrumentResults.Add(GetResultFromScript(modelPath, scriptName.Name,
                            scriptName.InstrumentName, report));
                    }

                    report.ModelResults.Add(modelResult);
                }
            }
        }

        private ModelInstrumentResult GetResultFromScript(string modelPath, string name, string instrumentName,
            Report report)
        {
            var strategy = _providedStrategies.FirstOrDefault(strategy => strategy.GetName() == instrumentName);
            if (strategy == null) return null;

            report.SummaryReport.TotalModelsCount++;
            var validationModelResult = strategy.ValidateModel(modelPath);
            if (validationModelResult.Result == Constants.Success)
            {
                report.SummaryReport.SucceededModelsCount++;
            }
            else
            {
                report.SummaryReport.FailedModelsCount++;
            }

            validationModelResult.ScriptName = name;
            return validationModelResult;
        }
    }
}