using ReportEngine.models;

namespace ReportEngine.strategies.interfaces
{
    public interface IInstrumentStrategy
    {
        void Init(string path);
        ResultInfo ValidateModel(string path, string modelPath);
        string GetName();
    }
}