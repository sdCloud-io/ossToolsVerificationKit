using System.Collections.Generic;

namespace ReportEngine.models
{
    public class Configuration
    {
        public List<ModelConfiguration> Models { get; set; }
        public List<InstrumentConfiguration> Instruments { get; set; }
        public List<ScriptConfiguration> Scripts { get; set; }
        public string BuildDir { get; set; }
        public string ReportFilePath { get; set; }
        public string ComparisonReportFilePath { get; set; }
        public double ConfidenceInterval { get; set; }
    }
}