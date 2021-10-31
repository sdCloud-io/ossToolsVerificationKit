using System.Collections.Generic;
using System.Linq;
using ReportEngine.services;
using ReportEngine.strategies.interfaces;

namespace ReportEngine.strategies
{
    public class StrategyProvider : IStrategyProvider
    {
        private readonly IEnumerable<IInstrumentStrategy> _strategies;

        public StrategyProvider(IEnumerable<IInstrumentStrategy> strategies)
        {
            _strategies = strategies;
        }

        public List<IInstrumentStrategy> GetAllStrategies()
        {
            var strategyNames =
                ReportBuilder.Configuration.Instruments.Select(instrument => instrument.Name);
            var requestedStrategies =
                _strategies.Where(strategy => strategyNames.Contains(strategy.GetName())).ToList();

            foreach (var strategy in requestedStrategies)
            {
                var path = ReportBuilder.Configuration.Instruments.FirstOrDefault(i => i.Name == strategy.GetName())
                    ?.Path;
                strategy.Init(path);
            }

            return requestedStrategies;
        }
    }
}