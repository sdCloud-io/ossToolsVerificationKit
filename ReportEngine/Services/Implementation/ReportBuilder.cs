using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportEngine.Configuration;
using ReportEngine.FileSystem;
using ReportEngine.GitHelper;
using ReportEngine.InstrumentHelper;
using ReportEngine.models;

namespace ReportEngine.Services.Implementation
{
    public class ReportBuilder : IReportBuilder
    {
        private readonly IFileSystemHelper _fileSystemHelper;
        private readonly IGitHelper _gitHelper;
        private readonly IInstrumentProvider _instrumentProvider;
        private readonly ILogger<ReportBuilder> _logger;
        private readonly Configuration.Configuration _configuration;

        public ReportBuilder(IFileSystemHelper fileSystemHelper,
            IOptions<Configuration.Configuration> configuration,
            IGitHelper gitHelper, IInstrumentProvider instrumentProvider, ILogger<ReportBuilder> logger)
        {
            _fileSystemHelper = fileSystemHelper;
            _gitHelper = gitHelper;
            _instrumentProvider = instrumentProvider;
            _logger = logger;
            _configuration = configuration.Value;
        }

        public void GenerateExecutionReport()
        {
            foreach (var instrumentConfiguration in _configuration.Instruments)
            {
                var reports = ExecuteInstruments(instrumentConfiguration).ToList();
                foreach (var report in reports)
                {
                    var reportJson = JsonSerializer.Serialize(report);
                    var fileName =
                        $"{instrumentConfiguration.Name}_{report.ModelTypes}_{report.InstrumentVersion}.json";
                    _gitHelper.SendFileAsync(fileName, reportJson).Wait();
                }
            }
        }

        public IEnumerable<Report> ExecuteInstruments(InstrumentConfiguration instrumentConfiguration)
        {
            foreach (var fileExtension in instrumentConfiguration.Models)
            {
                var timer = new Stopwatch();
                timer.Start();

                var report = CreateReport(fileExtension, instrumentConfiguration);
                report.InstrumentVersion =
                    _gitHelper.GetTag(instrumentConfiguration.Path);
                RunAllModelsByExtension(fileExtension, instrumentConfiguration, report);

                report.SummaryReport.TotalExecutionTime = timer.ElapsedMilliseconds;
                timer.Stop();
                yield return report;
            }
        }

        private Report CreateReport(string fileExtension, InstrumentConfiguration instrumentConfiguration)
        {
            var modelType = fileExtension.Contains(".xmile") ? ModelType.XMILE : ModelType.VENSIM;

            return new Report
            {
                ModelTypes = modelType.ToString(),
                InstrumentName = instrumentConfiguration.Name
            };
        }

        private void RunAllModelsByExtension(string fileExtension, InstrumentConfiguration instrumentConfiguration,
            Report report)
        {
            foreach (var modelPath in _fileSystemHelper.GetModelPathsByExtension(fileExtension))
            {
                RunModelOnInstrument(modelPath, instrumentConfiguration, report);
            }
        }

        private void RunModelOnInstrument(string modelPath, InstrumentConfiguration instrumentConfiguration,
            Report report)
        {
            var instrument = GetInstrumentByName(instrumentConfiguration.Name);
            if (instrument == null) return;

            report.SummaryReport.TotalModelsCount++;
            var validationModelResult = instrument.ExecuteModel(modelPath);
            var modelResult = new ModelResult
            {
                ModelPath = modelPath,
                CodeCompilationTime = validationModelResult.CodeCompilationTime,
                CodeExecutionTime = validationModelResult.CodeExecutionTime,
                CodeGenerationTime = validationModelResult.CodeGenerationTime,
                ResultDictionary = validationModelResult.ResultDictionary,
                Log = validationModelResult.Log,
                Result = validationModelResult.Result
            };

            UpdateSummaryResult(validationModelResult.Result, report.SummaryReport);
            report.ModelResults.Add(modelResult);
        }

        private Instruments.IInstrumentExecutor GetInstrumentByName(string instrumentName)
        {
            return _instrumentProvider.GetInstrument(instrumentName);
        }

        private void UpdateSummaryResult(string result, SummaryReport summaryReport)
        {
            if (result == Constants.Success)
            {
                summaryReport.SucceededModelsCount++;
            }
            else
            {
                summaryReport.FailedModelsCount++;
            }
        }
    }
}