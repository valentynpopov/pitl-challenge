using PITL.Extract.Job.Abstractions;
using PITL.Extract.Job.Abstractions.Input;
using System;

namespace PITL.Extract.Job.Input
{
    public class Clock : IClock
    {
        DateTime IClock.UtcNow => DateTime.UtcNow;
    }
}
