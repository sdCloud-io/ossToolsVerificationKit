using System;
using System.Collections.Generic;

namespace ReportEngine.models
{
    public class Report
    {
        public string InstrumentName { get; set; }
        public string ModelTypes { get; set; }
        public List<ModelResult> ModelResults { get; set; } = new();
        public SummaryReport SummaryReport { get; set; } = new();
        public DateTime StartTime { get; } = DateTime.Now;
        public string InstrumentVersion { get; set; }
    }
}