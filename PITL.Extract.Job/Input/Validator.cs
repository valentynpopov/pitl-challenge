using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PITL.Extract.Job.Abstractions;
using Services;

namespace PITL.Extract.Job.Input
{
    public class Validator : IValidator
    {
        private readonly ILogger<Validator> _logger;

        public Validator(ILogger<Validator> logger)
        {
            _logger = logger;
        }

        public bool IsValidResponse(DateTime extractDateTime, IReadOnlyList<PowerTrade> trades)
        {
            if (trades.Count == 0)
                return true;

            foreach(var trade in trades)
            {
                if (!IsValidTrade(extractDateTime, trade)) 
                    return false;
            }
            return true;
        }

        private bool IsValidTrade(DateTime extractDateTime, PowerTrade trade)
        {
            if (trade.Date.Date != extractDateTime.Date)
            {
                _logger.LogError("Trade date {tradeDate} does not match extract date {extractDateTime}", 
                    trade.Date.Date, extractDateTime.Date);
                return false;
            }

            var periods = trade.Periods.Select(p => p.Period).ToArray();

            var duplicateCount = periods.Length - periods.Distinct().Count();
            if (duplicateCount > 0)
            {
                _logger.LogError("Found {duplicateCount} duplicate periods", duplicateCount);
                return false;
            }

            if (trade.Date != extractDateTime)
                _logger.LogWarning("Trade time {tradeDate} does not match extract time {extractDateTime}", 
                    trade.Date, extractDateTime);

            var nonStandardPeriods = periods.Where(period => period < 1 || period > 24);
            _logger.LogWarning("Non-standard periods found: {periods}", string.Join(',', nonStandardPeriods));

            if (periods.Length < 24)
                _logger.LogWarning("Normally 24 periods are expected, but only {periods} found", periods.Length);

            var isIncreasingStrictlyMonotonically = periods.Zip(
                periods.Skip(1), (a, b) => a.CompareTo(b) < 0).All(b => b);
            if (!isIncreasingStrictlyMonotonically)
                _logger.LogWarning("Normally periods are expected to be pre-sorted");

            return true;
        }
    }
}
