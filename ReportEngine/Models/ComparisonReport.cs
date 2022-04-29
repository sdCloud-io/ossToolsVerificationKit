using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ReportEngine.models
{
    public class ComparisonReport
    {
        public ConcurrentBag<ComparisonModel> ComparisonModels { get; set; } = new ConcurrentBag<ComparisonModel>();
    }
}