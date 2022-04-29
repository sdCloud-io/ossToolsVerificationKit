using System.Collections.Generic;

namespace ReportEngine.models
{
    public class ComparisonInstrument
    {
        public string Name { get; set; }
        public Dictionary<string, bool> Values { get; set; }
    }
}