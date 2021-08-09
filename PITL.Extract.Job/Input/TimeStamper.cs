using PITL.Extract.Job.Abstractions.Input;
using System;

namespace PITL.Extract.Job.Input
{
    public class TimeStamper : ITimeStamper
    {
        public DateTime ConvertPeriodToTime(DateTime extractDate, int period)
        {
            if (period < 1 || period > 24)
                throw new ArgumentException($"Period {period} was expected to be between 1 and 24");

            var d = extractDate.AddHours(period - 2);
            return d;
        }
    }
}
