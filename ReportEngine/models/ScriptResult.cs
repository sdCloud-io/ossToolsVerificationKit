using System.Collections.Generic;

namespace ReportEngine.models
{
    public class ScriptResult
    {
        public string ScriptName { get; set; }
        public List<ResultInfo> Results { get; set; } = new();
    }
}