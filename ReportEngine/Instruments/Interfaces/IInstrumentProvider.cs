using System.Collections.Generic;
using ReportEngine.strategies.interfaces;

namespace ReportEngine.Instruments.Interfaces
{
    public interface IInstrumentProvider
    {
        List<IInstrumentExecutor> GetAllStrategies();
    }
}