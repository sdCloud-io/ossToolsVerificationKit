using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportEngine.models;

namespace ReportEngine.Services.Implementation
{
    public class ScriptComparator : IScriptComparator
    {
        private readonly Configuration _configuration;
        private readonly ILogger<ReportBuilder> _logger;

        public ScriptComparator(ILogger<ReportBuilder> logger, IOptions<Configuration> configuration)
        {
            _configuration = configuration.Value;
            _logger = logger;
        }

        public ComparisonReport CompareResults(Report report)
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
    }
}