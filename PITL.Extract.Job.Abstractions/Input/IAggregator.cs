using Services;
using System.Collections.Generic;

namespace PITL.Extract.Job.Abstractions.Input
{
    public interface IAggregator
    {
        PowerPeriod[] GetAggregatedVolumes(IReadOnlyList<PowerTrade> trades);
    }
}
