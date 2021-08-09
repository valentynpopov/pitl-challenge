using Microsoft.Extensions.Logging;
using PITL.Extract.Job.Abstractions.Input;
using Services;
using System;
using System.Collections.Generic;
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
        private readonly DateTime _timeToGiveUp;
        private readonly Stopwatch _stopwatch = new();

        public FailSafeSource(IPowerService powerService, IClock clock, ILogger<FailSafeSource> logger, DateTime timeToGiveUp)
        {
            _powerService = powerService;
            _clock = clock;
            _logger = logger;
            _timeToGiveUp = timeToGiveUp;
        }

        private bool TryGetTrades(DateTime date, out PowerTrade[] trades)
        {
            _stopwatch.Restart();
            try
            {
                _logger.LogInformation("Getting trades from PowerService...");
                trades = _powerService.GetTrades(date).ToArray();
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

        public PowerTrade[] GetTradesOrEmpty(DateTime date, CancellationToken stoppingToken)
        {
            do
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Cancellation requested");
                    return Array.Empty<PowerTrade>();
                }

                if (TryGetTrades(date, out var trades))
                    return trades;                
            }
            while (_clock.UtcNow < _timeToGiveUp);

            _logger.LogError("Could not get a response from PowerService in time, we will assume there are no trades");
            return Array.Empty<PowerTrade>();
        }
    }
}
