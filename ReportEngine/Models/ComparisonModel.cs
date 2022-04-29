using System.Collections.Generic;

namespace ReportEngine.models
{
    public class ComparisonModel
    {
        public string ModelPath { get; set; }
        public List<ComparisonInstrument> ComparisonInstruments { get; set; }
    }
}