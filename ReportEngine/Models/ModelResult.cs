using System.Collections.Generic;

namespace ReportEngine.models
{
    public class ModelResult
    {
        public string ModelPath { get; set; }

        public List<ModelInstrumentResult> ModelInstrumentResults { get; set; } = new();
    }
}