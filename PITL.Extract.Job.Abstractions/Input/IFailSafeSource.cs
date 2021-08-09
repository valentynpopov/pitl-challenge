using Services;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PITL.Extract.Job.Abstractions.Input
{
    public interface IFailSafeSource
    {
        PowerTrade[] GetTradesOrEmpty(DateTime date, CancellationToken stoppingToken);
    }
}
