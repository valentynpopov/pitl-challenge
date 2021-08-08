using System;

namespace PITL.Extract.Job.Abstractions
{
    public record Position
    (
        DateTime LocalTime,
        int Volume
    );
}
