namespace ReportEngine.models
{
    public class SummaryReport
    {
        public long TotalExecutionTime { get; set; }
        public long TotalModelsCount { get; set; }
        public long SucceededModelsCount { get; set; }
        public long FailedModelsCount { get; set; }
    }
}