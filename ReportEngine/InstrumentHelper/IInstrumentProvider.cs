using ReportEngine.Instruments;

namespace ReportEngine.InstrumentHelper
{
    public interface IInstrumentProvider
    {
        IInstrumentExecutor GetInstrument(string instrumentName);
    }
}