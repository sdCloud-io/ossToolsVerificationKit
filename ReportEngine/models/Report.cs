using System;
using System.Collections.Generic;

namespace ReportEngine.models
{
    public class Report
    {
        public double StartTimeStamp { get; set; } = DateTime.Now.ToFileTime();
        public DateTime StartTime { get; set; } = new DateTime();
        public List<ScriptResult> ScriptResults { get; set; } = new();
        public SummaryReport SummaryReport { get; set; }
    }
}