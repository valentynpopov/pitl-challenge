using System;
using System.ComponentModel.DataAnnotations;

namespace PITL.Extract.Job.Abstractions
{
    public record ExtractOptions
    {
        public string OutputPath { get; init; }

        [Range(1, 1440, ErrorMessage = "Interval {0} must be between {1} and {2} minutes (i.e. no more than a day)")]
        public int IntervalInMinutes { get; init; }

        public TimeSpan TimeToGiveUp { get; init; }
    }
}
