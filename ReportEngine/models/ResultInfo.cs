namespace ReportEngine.models
{
    public class ResultInfo
    {
        public string Result { get; set; }
        public string ModelPath { get; set; }
        public string Log { get; set; }
        public long CodeGenerationTime { get; set; }
        public long CodeCompilationTime { get; set; }
        public long CodeExecutionTime { get; set; }
    }
}