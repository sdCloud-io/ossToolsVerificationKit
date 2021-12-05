using System;
using System.Collections.Generic;

namespace ReportEngine.models
{
    public class Report
    {
        public double StartTimeStamp { get; } = DateTime.Now.ToFileTime();
        public DateTime StartTime { get; } = DateTime.Now;
        public List<ScriptResult> ScriptResults { get; set; } = new();
        public SummaryReport SummaryReport { get; set; } = new SummaryReport();
    }
}