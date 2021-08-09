using Microsoft.Extensions.Logging;
using PITL.Extract.Job.Abstractions.Input;
using Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PITL.Extract.Job.Input
{
    public class Aggregator : IAggregator
    {
        private readonly ILogger<Aggregator> _logger;
        private readonly Stopwatch _stopwatch = new();

        public Aggregator(ILogger<Aggregator> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public PowerPeriod[] GetAggregatedVolumes(IReadOnlyList<PowerTrade> trades)
        {
            _logger.LogInformation("Aggregating {tradeCount} trades...", trades.Count);
            _stopwatch.Restart();

            var aggregated = trades
                .SelectMany(trade => trade.Periods)
                // ignore non-standard periods
                .Where(period => period.Period >= 1 && period.Period <= 24)
                .GroupBy(period => period.Period)
                // PowerService might've returned unordered periods
                .OrderBy(group => group.Key)
                .Select(group => new PowerPeriod
                {
                    Period = group.Key,
                    Volume = group.Sum(x => x.Volume)
                });

            _stopwatch.Stop();
            _logger.LogInformation("Aggregated {tradeCount} trades in {elapsed} ms", trades.Count, _stopwatch.ElapsedMilliseconds);
            return aggregated.ToArray();
        }
    }
}
