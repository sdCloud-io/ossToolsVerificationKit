using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using ReportEngine.Instruments.Interfaces;
using ReportEngine.models;
using ReportEngine.strategies.interfaces;

namespace ReportEngine.Instruments
{
    public class InstrumentsProvider : IInstrumentProvider
    {
        private readonly IEnumerable<IInstrumentExecutor> _strategies;
        private readonly Configuration _configuration;

        public InstrumentsProvider(IEnumerable<IInstrumentExecutor> strategies, IOptions<Configuration> configuration)
        {
            _strategies = strategies;
            _configuration = configuration.Value;
        }

        public List<IInstrumentExecutor> GetAllStrategies()
        {
            var strategyNames =
                _configuration.Instruments.Select(instrument => instrument.Name);
            var requestedStrategies =
                _strategies.Where(strategy => strategyNames.Contains(strategy.GetName())).ToList();

            foreach (var strategy in requestedStrategies)
            {
                var path = _configuration.Instruments.FirstOrDefault(i => i.Name == strategy.GetName())
                    ?.Path;
                strategy.Init(path);
            }

            return requestedStrategies;
        }
    }
}