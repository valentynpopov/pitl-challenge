using System;
using System.Collections.Generic;
using System.Threading;

namespace PITL.Extract.Job.Abstractions.Input
{
    public interface IPositionReader
    {
        Position[] GetPositionsExtract(DateTime extractTime, CancellationToken stoppingToken);
    }
}
