using System.Collections.Generic;

namespace ReportEngine.Configuration
{
    public class Configuration
    {
        public List<string> Models { get; set; }
        public List<InstrumentConfiguration> Instruments { get; set; }
        public string BuildDir { get; set; }
        public GitConfiguration GitConfiguration { get; set; }
    }
}