using Microsoft.Extensions.Logging;
using PITL.Extract.Job.Abstractions;
using PITL.Extract.Job.Abstractions.Input;
using Services;
using System;
using System.Linq;
using System.Threading;

namespace PITL.Extract.Job.Input
{
    public class PositionReader : IPositionReader
    {
        private readonly IFailSafeSource _failSafeSource;
        private readonly IAggregator _aggregator;
        private readonly IValidator _validator;
        private readonly ITimeStamper _timeStamper;
        private readonly ILogger<PositionReader> _logger;

        public PositionReader(IFailSafeSource failSafeSource, IAggregator aggregator, IValidator validator, ITimeStamper timeStamper, ILogger<PositionReader> logger)
        {
            _failSafeSource = failSafeSource ?? throw new ArgumentNullException(nameof(failSafeSource));
            _aggregator = aggregator ?? throw new ArgumentNullException(nameof(aggregator));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _timeStamper = timeStamper ?? throw new ArgumentNullException(nameof(timeStamper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
        }

        public Position[] GetPositionsExtract(DateTime extractTime, CancellationToken stoppingToken)
        {
            // we only need the date portion
            var extractDate = extractTime.Date;

            _logger.LogInformation("Getting trades for {extractDate}...", extractDate);
            var trades = _failSafeSource.GetTradesOrEmpty(extractDate, stoppingToken);

            _logger.LogInformation("Validating trades...");
            if (!_validator.IsValidResponse(extractDate, trades))
            {
                trades = Array.Empty<PowerTrade>();
                _logger.LogWarning("As invalid trades have been returned, assuming there were no trades");
            }

            _logger.LogInformation("Aggregating trades...");
            var aggregated = _aggregator.GetAggregatedVolumes(trades);

            _logger.LogInformation("Converting periods into timestamps...");
            var timestamped = (from a in aggregated
                              let localTime = _timeStamper.ConvertPeriodToTime(extractDate, a.Period)
                              select new Position(localTime, a.Volume)).ToArray();

            _logger.LogInformation("Positions are ready");
            return timestamped;
        }
    }
}
