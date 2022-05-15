using System.Collections.Generic;
using System.Linq;
using ReportEngine.Instruments;

namespace ReportEngine.InstrumentHelper
{
    public class InstrumentsProvider : IInstrumentProvider
    {
        private readonly IEnumerable<IInstrumentExecutor> _instrumentExecutors;

        public InstrumentsProvider(IEnumerable<IInstrumentExecutor> instrumentExecutors)
        {
            _instrumentExecutors = instrumentExecutors;
        }

        public IInstrumentExecutor GetInstrument(string instrumentName)
        {
            return _instrumentExecutors.FirstOrDefault(instrument => instrument.Name == instrumentName);
        }
    }
}