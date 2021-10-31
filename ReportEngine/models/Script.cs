using System.Collections.Generic;

namespace ReportEngine.models
{
    public class Script
    {
        public string Name { get; set; }
        public string InstrumentName { get; set; }
        public List<string> ModelPaths { get; set; }
        public List<string> ExtModels { get; set; }
        public string PathResult { get; set; }
    }
}