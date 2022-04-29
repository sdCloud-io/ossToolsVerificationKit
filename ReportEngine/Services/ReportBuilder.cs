using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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
        private readonly IFileSystemHelper _fileSystemHelper;
        private readonly Configuration _configuration;
        private readonly List<IInstrumentStrategy> _providedStrategies;
        private readonly ILogger<ReportBuilder> _logger;

        public ReportBuilder(ILogger<ReportBuilder> logger, IFileSystemHelper fileSystemHelper,
            IStrategyProvider strategyProvider, IOptions<Configuration> configuration)
        {
            _fileSystemHelper = fileSystemHelper;
            _configuration = configuration.Value;

            _logger = logger;
            logger.LogInformation("Setting up build directory");
            _fileSystemHelper.ChangeDirectory(_configuration.BuildDir);

            _providedStrategies = strategyProvider.GetAllStrategies();
        }

        public Report GenerateReport()
        {
            var timer = new Stopwatch();
            timer.Start();
            var report = new Report();

            ExecuteScripts(_configuration.Scripts, report);
            report.SummaryReport.TotalExecutionTime = timer.ElapsedMilliseconds;
            _fileSystemHelper.WriteJsonInFile(report, _configuration.ReportFilePath);

            return report;
        }

        public ComparisonReport GenerateComparisonReport(Report report)
        {
            var comparisonReport = CompareResults(report);
            _fileSystemHelper.WriteJsonInFile(comparisonReport, _configuration.ComparisonReportFilePath);

            return comparisonReport;
        }

        private ComparisonReport CompareResults(Report report)
        {
            _logger.LogInformation("Start compare");
            var comparisonReport = new ComparisonReport();
            Parallel.ForEach(report.ModelResults, result =>
            {
                _logger.LogInformation(result.ModelPath);
                if (!IsAllInstrumentSuccess(result.ModelInstrumentResults)) return;
                var resultDictionaries = result.ModelInstrumentResults
                    .Select(instrumentResult => instrumentResult.ResultDictionary)
                    .ToList();
                var instruments = result.ModelInstrumentResults
                    .Select(instrumentResult => instrumentResult.ScriptName)
                    .ToList();

                var comparisonInstruments = CompareResultDictionaries(instruments, resultDictionaries)
                    .Select(comparison => new ComparisonInstrument
                    {
                        Name = comparison.Key,
                        Values = comparison.Value
                    })
                    .ToList();

                comparisonReport.ComparisonModels.Add(new ComparisonModel
                {
                    ModelPath = result.ModelPath,
                    ComparisonInstruments = comparisonInstruments
                });
            });

            return comparisonReport;
        }

        private Dictionary<string, Dictionary<string, bool>> CompareResultDictionaries(List<string> scripts,
            List<Dictionary<string, List<string>>> values)
        {
            var parameters = GetUniqueParametersOfResultDictionaries(values);
            var confidenceIntervalScripts = new Dictionary<string, Dictionary<string, bool>>();

            for (var i = 0; i < values.Count - 1; i++)
            {
                for (var j = i + 1; j < values.Count; j++)
                {
                    var compareKey = string.Join(':', scripts[i], scripts[j]);
                    confidenceIntervalScripts[compareKey] = new Dictionary<string, bool>();
                    foreach (var parameter in parameters)
                    {
                        var firstValues = values[i][parameter];
                        var secondValues = values[i][parameter];

                        try
                        {
                            var confidenceIntervalParameter = CompareValues(firstValues, secondValues);
                            if (confidenceIntervalParameter <= _configuration.ConfidenceInterval)
                            {
                                confidenceIntervalScripts[compareKey][parameter] = true;
                            }
                            else
                            {
                                confidenceIntervalScripts[compareKey][parameter] = false;
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogInformation(e.Message);
                        }
                    }
                }
            }

            return confidenceIntervalScripts;
        }

        //Compare end state of models
        private double CompareValues(List<string> firstValues, List<string> secondValues)
        {
            var firstValueDouble = JsonSerializer.Deserialize<double>(firstValues.LastOrDefault());
            var secondValueDouble = JsonSerializer.Deserialize<double>(secondValues.LastOrDefault());
            return GetDifferenceInPercent(firstValueDouble, secondValueDouble);
        }

        private double GetDifferenceInPercent(double firstValueDouble, double secondValueDouble)
        {
            if (firstValueDouble < secondValueDouble)
            {
                return (secondValueDouble - firstValueDouble) / firstValueDouble * 100;
            }

            return (secondValueDouble - firstValueDouble) / firstValueDouble * 100;
        }

        private List<string> GetUniqueParametersOfResultDictionaries(List<Dictionary<string, List<string>>> values)
        {
            return values.Select(value => value.Keys)
                .Aggregate(values.First().Keys.ToList(),
                    (current, keysOfValues) => current.Intersect(keysOfValues).ToList());
        }

        private bool IsAllInstrumentSuccess(List<ModelInstrumentResult> resultModelInstrumentResults)
        {
            return
                resultModelInstrumentResults != null &&
                resultModelInstrumentResults.All(result => result != null) &&
                resultModelInstrumentResults.All(result => result.IsSuccess()) &&
                resultModelInstrumentResults.Count > 1;
        }

        private void ExecuteScripts(IEnumerable<ScriptConfiguration> scripts,
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