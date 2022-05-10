using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReportEngine.filesystem.interfaces;
using ReportEngine.Instruments.Interfaces;
using ReportEngine.models;
using ReportEngine.strategies.interfaces;

namespace ReportEngine.Services.Implementation
{
    public class ScriptExecution : IScriptExecution
    {
        private readonly IFileSystemHelper _fileSystemHelper;
        private readonly List<IInstrumentExecutor> _instruments;

        public ScriptExecution(ILogger<ScriptExecution> logger, IFileSystemHelper fileSystemHelper,
            IInstrumentProvider instrumentProvider, IOptions<Configuration> configuration)
        {
            _fileSystemHelper = fileSystemHelper;

            logger.LogInformation("Setting up build directory");
            _fileSystemHelper.ChangeDirectory(configuration.Value.BuildDir);

            _instruments = instrumentProvider.GetAllStrategies();
        }

        public Report ExecuteScripts(IEnumerable<ScriptConfiguration> scripts)
        {
            var timer = new Stopwatch();
            timer.Start();
            var report = new Report();

            var fileExtensions = scripts.GroupBy(script => script.ExtModels).Select(group => new
            {
                FileExtension = group.Key,
                ScriptNames = group.Select(script => new ScriptInformation
                {
                    Name = script.Name,
                    InstrumentName = script.InstrumentName
                })
            });

            foreach (var fileExtension in fileExtensions)
            {
                RunAllModelsByExtension(fileExtension.FileExtension, fileExtension.ScriptNames.ToList(), report);
            }

            report.SummaryReport.TotalExecutionTime = timer.ElapsedMilliseconds;
            timer.Stop();
            return report;
        }

        private void RunAllModelsByExtension(string fileExtension, IList<ScriptInformation> scriptsInformation,
            Report report)
        {
            foreach (var modelPath in _fileSystemHelper.GetModelPathsByExtension(fileExtension))
            {
                RunModelOnInstruments(modelPath, scriptsInformation, report);
            }
        }

        private void RunModelOnInstruments(string modelPath, IList<ScriptInformation> scriptsInformation, Report report)
        {
            var modelResult = new ModelResult
            {
                ModelPath = modelPath,
            };

            foreach (var scriptInformation in scriptsInformation)
            {
                var modelInstrumentResult = GetResultFromScript(modelPath, scriptInformation.Name,
                    scriptInformation.InstrumentName, report);
                modelResult.ModelInstrumentResults.Add(modelInstrumentResult);
            }

            report.ModelResults.Add(modelResult);
        }

        private ModelInstrumentResult GetResultFromScript(string modelPath, string name, string instrumentName,
            Report report)
        {
            var instrument = GetInstrumentByName(instrumentName);
            if (instrument == null) return null;

            report.SummaryReport.TotalModelsCount++;
            var validationModelResult = instrument.ValidateModel(modelPath);

            WriteValidationModelResult(validationModelResult.Result, report.SummaryReport);

            validationModelResult.ScriptName = name;
            return validationModelResult;
        }

        private IInstrumentExecutor GetInstrumentByName(string instrumentName)
        {
            return _instruments.FirstOrDefault(instrument => instrument.GetName() == instrumentName);
        }

        private void WriteValidationModelResult(string result, SummaryReport summaryReport)
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