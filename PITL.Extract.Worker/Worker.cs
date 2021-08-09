using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PITL.Extract.Job;
using PITL.Extract.Job.Abstractions.Input;
using PITL.Extract.Job.Abstractions.Output;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _logger = logger;
            _positionReader = positionReader;
            _csvFileCreator = csvFileCreator;
        }

        private ExtractJob CreateJob() => new ExtractJob(_positionReader, _csvFileCreator);

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
