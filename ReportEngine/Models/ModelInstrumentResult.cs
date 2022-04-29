using System.Collections.Generic;
using System.Text.Json.Serialization;

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

        [JsonIgnore] public Dictionary<string, List<string>> ResultDictionary { get; set; }

        public bool IsSuccess()
        {
            return Result == Constants.Success;
        }
    }
}