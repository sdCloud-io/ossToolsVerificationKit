using ReportEngine.models;

namespace ReportEngine.Services
{
    public interface IReportBuilder
    {
        Report GenerateExecutionReport();
        ComparisonReport GenerateComparisonReport(Report report);
    }
}