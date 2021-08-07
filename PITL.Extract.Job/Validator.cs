using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Services;

namespace PITL.Extract.Job
{
    public class Validator
    {
        private readonly ILogger<Validator> _logger;

        public Validator(ILogger<Validator> logger)
        {
            _logger = logger;
        }

        public bool IsValidResponse(DateTime requestDate, IReadOnlyList<PowerTrade> trades)
        {
            if (!trades.Any())
            {
                _logger.LogInformation("No trades this time");
                return true;
            }

            foreach(var trade in trades)
            {
                if (!IsValidTrade(requestDate, trade)) 
                    return false;
            }
            return true;
        }

        private bool IsValidTrade(DateTime requestDate, PowerTrade trade)
        {
            if (trade.Date != requestDate)
            {
                _logger.LogError("Trade date/time {tradeDate} does not match request date/time {requestDate}", trade.Date, requestDate);
                return false;
            }
            
            var periods = trade.Periods.Select(p => p.Period).ToArray();
            
            if (periods.Min() < 1)
            {
                _logger.LogError("First period should be 1");
                return false;
            }

            if (periods.Max() > 24)
            {
                _logger.LogError("Last period should be 24");
                return false;
            }

            if (periods.Length > periods.Distinct().Count())
            {
                _logger.LogError("Duplicate periods found");
                return false;
            }

            if (periods.Length < 24)
                _logger.LogWarning("FYI, normally 24 periods are expected, but only {periods} found", periods.Length);

            var isIncreasingStrictlyMonotonically = periods.Zip(
                periods.Skip(1), (a, b) => a.CompareTo(b) < 0).All(b => b);
            if (!isIncreasingStrictlyMonotonically)
                _logger.LogWarning("FYI, normally periods are expected to be pre-sorted");

            return true;
        }
    }
}
