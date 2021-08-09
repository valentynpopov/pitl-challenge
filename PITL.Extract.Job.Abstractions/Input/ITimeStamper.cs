using System;

namespace PITL.Extract.Job.Abstractions.Input
{
    public interface ITimeStamper
    {
        DateTime ConvertPeriodToTime(DateTime extractDate, int period);
    }
}
