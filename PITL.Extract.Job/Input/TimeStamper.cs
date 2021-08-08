using System;

namespace PITL.Extract.Job.Input
{
    public class TimeStamper
    {
        public DateTime ConvertPeriodToTime(int period)
        {
            if (period < 1 || period > 24)
                throw new ArgumentException($"Period {period} was expected to be between 1 and 24");

            // In .NET 6 we would've been able to use TimeOnly
            var time = new DateTime(0, 0, 0, period, 0, 0);
            time = time.AddHours(-2);
            return time;
        }
    }
}
