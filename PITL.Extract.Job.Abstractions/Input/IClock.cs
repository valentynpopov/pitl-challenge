using System;

namespace PITL.Extract.Job.Abstractions.Input
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
