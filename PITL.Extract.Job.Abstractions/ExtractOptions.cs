using System;

namespace PITL.Extract.Job.Abstractions
{
    public record ExtractOptions
    {
        public string OutputPath { get; init; }
        public int IntervalInMinutes { get; init; }
        public TimeSpan TimeToGiveUp { get; init; }
    }
}
