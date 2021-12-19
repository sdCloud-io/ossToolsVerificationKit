using ReportEngine.models;

namespace ReportEngine.strategies.interfaces
{
    public interface IInstrumentStrategy
    {
        void Init(string path);
        ModelInstrumentResult ValidateModel(string path);
        string GetName();
    }
}