using System.Collections.Generic;

namespace ReportEngine.strategies.interfaces
{
    public interface IStrategyProvider
    {
        List<IInstrumentStrategy> GetAllStrategies();
    }
}