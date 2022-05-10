using Microsoft.Extensions.Options;
using ReportEngine.filesystem.interfaces;
using ReportEngine.models;

namespace ReportEngine.Services.Implementation
{
    public class ReportBuilder : IReportBuilder
    {
        private readonly IFileSystemHelper _fileSystemHelper;
        private readonly IScriptComparator _scriptComparator;
        private readonly IScriptExecution _scriptExecution;
        private readonly Configuration _configuration;

        public ReportBuilder(IFileSystemHelper fileSystemHelper,
            IOptions<Configuration> configuration,
            IScriptComparator scriptComparator, IScriptExecution scriptExecution)
        {
            _fileSystemHelper = fileSystemHelper;
            _scriptComparator = scriptComparator;
            _scriptExecution = scriptExecution;
            _configuration = configuration.Value;
        }

        public Report GenerateExecutionReport()
        {
            var report = _scriptExecution.ExecuteScripts(_configuration.Scripts);
            _fileSystemHelper.WriteJsonInFile(report, _configuration.ReportFilePath);

            return report;
        }

        public ComparisonReport GenerateComparisonReport(Report report)
        {
            var comparisonReport = _scriptComparator.CompareResults(report);
            _fileSystemHelper.WriteJsonInFile(comparisonReport, _configuration.ComparisonReportFilePath);

            return comparisonReport;
        }
    }
}