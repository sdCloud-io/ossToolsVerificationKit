using System.Collections.Generic;

namespace ReportEngine.models
{
    public class ModelInstrumentResult
    {
        public string ScriptName { get; set; }
        public string Result { get; set; } = Constants.Failed;
        public string Log { get; set; }
        public long CodeGenerationTime { get; set; }
        public long CodeCompilationTime { get; set; }
        public long CodeExecutionTime { get; set; }
        public Dictionary<string, List<string>> ResultDictionary { get; set; }
    }
}