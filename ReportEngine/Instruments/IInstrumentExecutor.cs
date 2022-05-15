using ReportEngine.models;

namespace ReportEngine.Instruments
{
    public interface IInstrumentExecutor
    {
        ModelInstrumentResult ExecuteModel(string path);
        public string Name { get; }
    }
}