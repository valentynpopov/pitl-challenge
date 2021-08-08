using PITL.Extract.Job.Abstractions;
using System;

namespace PITL.Extract.Job.Input
{
    public class Clock : IClock
    {
        DateTime IClock.UtcNow => DateTime.UtcNow;
    }
}
