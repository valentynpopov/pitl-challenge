using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PITL.Extract.Job;
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

        public Worker(ILogger<Worker> logger, IPositionReader positionReader, ICsvFileCreator csvFileCreator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _positionReader = positionReader ?? throw new ArgumentNullException(nameof(positionReader));
            _csvFileCreator = csvFileCreator ?? throw new ArgumentNullException(nameof(csvFileCreator));
        }

        private ExtractJob CreateJob() => new(_positionReader, _csvFileCreator);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = CreateJob();
                job.FireAndForget(DateTime.UtcNow, stoppingToken);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
