using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PITL.Extract.Job.Abstractions;
using PITL.Extract.Job.Abstractions.Input;
using Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PITL.Extract.Job.Input
{
    public class FailSafeSource : IFailSafeSource
    {
        private readonly IPowerService _powerService;
        private readonly IClock _clock;
        private readonly ILogger<FailSafeSource> _logger;
        private readonly TimeSpan _timeToGiveUp;
        private readonly Stopwatch _stopwatch = new();

        public FailSafeSource(IPowerService powerService, IClock clock, ILogger<FailSafeSource> logger, 
            IOptions<ExtractOptions> options)
        {
            _powerService = powerService ?? throw new ArgumentNullException(nameof(powerService));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (options is null)
                throw new ArgumentNullException(nameof(options));
            _timeToGiveUp = options.Value.TimeToGiveUp;
        }

        private bool TryGetTrades(DateTime extractDateTime, out PowerTrade[] trades)
        {
            _stopwatch.Restart();
            try
            {
                _logger.LogInformation("Getting trades from PowerService...");
                trades = _powerService.GetTrades(extractDateTime).ToArray();
                _stopwatch.Stop();

                if (trades == null)
                {
                    _logger.LogWarning("PowerService returned null response in {elapsed} ms, we will assume there are no trades",
                        _stopwatch.ElapsedMilliseconds);
                    trades = Array.Empty<PowerTrade>();
                }
                else 
                    _logger.LogInformation("PowerService returned {tradeCount} response in {elapsed} ms", 
                        trades.Length, _stopwatch.ElapsedMilliseconds);
                return true;
            }
            catch (PowerServiceException psex)
            {
                _logger.LogWarning("PowerService failed in {elapsed} ms with the message: {message}", 
                    _stopwatch.ElapsedMilliseconds, psex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("PowerService failed unexpectedly in {elapsed} ms with the message: {message}",
                    _stopwatch.ElapsedMilliseconds, ex.Message);
            }
            trades = null;
            return false;
        }

        public PowerTrade[] GetTradesOrEmpty(DateTime extractTime, CancellationToken stoppingToken)
        {
            var deadline = _clock.UtcNow + _timeToGiveUp;

            do
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Cancellation requested");
                    return Array.Empty<PowerTrade>();
                }

                if (TryGetTrades(extractTime.Date, out var trades))
                    return trades;                
            }
            while (_clock.UtcNow < deadline);

            _logger.LogError("Could not get a response from PowerService in time, we will assume there are no trades");
            return Array.Empty<PowerTrade>();
        }
    }
}
