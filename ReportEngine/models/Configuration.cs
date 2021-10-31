using System.Collections.Generic;

namespace ReportEngine.models
{
    public class Configuration
    {
        public List<Model> Models { get; set; }
        public List<Instrument> Instruments { get; set; }
        public string BuildDir { get; set; }
    }
}