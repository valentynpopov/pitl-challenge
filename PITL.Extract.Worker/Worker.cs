using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PITL.Extract.Job;
using PITL.Extract.Job.Abstractions;
using PITL.Extract.Job.Abstractions.Input;
using PITL.Extract.Job.Abstractions.Output;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PITL.Extract.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IPositionReader _positionReader;
        private readonly ICsvFileCreator _csvFileCreator;
        private readonly IClock _clock;
        private readonly int _intervalInMinutes;

        public Worker(IPositionReader positionReader, ICsvFileCreator csvFileCreator, 
            ILogger<Worker> logger, IClock clock, IOptions<ExtractOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _positionReader = positionReader ?? throw new ArgumentNullException(nameof(positionReader));
            _csvFileCreator = csvFileCreator ?? throw new ArgumentNullException(nameof(csvFileCreator));
            _clock = clock;

            if (options is null)
                throw new ArgumentNullException(nameof(options));
            _intervalInMinutes = options.Value.IntervalInMinutes;

            _logger.LogInformation("Interval between extracts is {interval} minutes", _intervalInMinutes);
            _logger.LogInformation("Give up calling PowerService after {timeToGiveUp}", options.Value.TimeToGiveUp);
            _logger.LogInformation("Output path for CSV file is ", options.Value.OutputPath);
        }

        private ExtractJob CreateJob() => new(_positionReader, _csvFileCreator);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var extractTime = _clock.UtcNow;

                var job = CreateJob();
                job.FireAndForget(extractTime, stoppingToken);

                var nextExtractTime = extractTime.AddMinutes(_intervalInMinutes);
                var delay = nextExtractTime - _clock.UtcNow;

                if (delay > TimeSpan.Zero)
                {
                    _logger.LogInformation("Waiting for {delay} until the next extract", delay);
                    await Task.Delay(delay, stoppingToken);
                }
            }
        }
    }
}
