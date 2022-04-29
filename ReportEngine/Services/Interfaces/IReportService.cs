using ReportEngine.models;

namespace ReportEngine.services.interfaces
{
    public interface IReportService
    {
        Report GenerateReport();
        ComparisonReport GenerateComparisonReport(Report report);
    }
}