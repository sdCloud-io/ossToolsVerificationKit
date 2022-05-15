using System.Collections.Generic;

namespace ReportEngine.models
{
    public class ModelInstrumentResult
    {
        public string Result { get; set; }
        public long CodeCompilationTime { get; set; }
        public long CodeExecutionTime { get; set; }
        public long CodeGenerationTime { get; set; }
        public string Log { get; set; }
        public Dictionary<string, string> ResultDictionary { get; set; }
    }
}