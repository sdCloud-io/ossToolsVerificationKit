using System.Collections.Generic;

namespace ReportEngine.Configuration
{
    public class InstrumentConfiguration
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public IEnumerable<string> Models { get; set; }
    }
}