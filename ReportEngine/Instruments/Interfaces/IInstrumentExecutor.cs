using ReportEngine.models;

namespace ReportEngine.strategies.interfaces
{
    public interface IInstrumentExecutor
    {
        void Init(string path);
        ModelInstrumentResult ValidateModel(string path);
        string GetName();
    }
}