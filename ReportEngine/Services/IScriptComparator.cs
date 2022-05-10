using ReportEngine.models;

namespace ReportEngine.Services
{
    public interface IScriptComparator
    {
        ComparisonReport CompareResults(Report report);
    }
}