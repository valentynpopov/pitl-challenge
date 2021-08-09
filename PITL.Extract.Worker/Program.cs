using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PITL.Extract.Job.Abstractions.Input;
using PITL.Extract.Job.Abstractions.Output;
using PITL.Extract.Job.Input;
using PITL.Extract.Job.Output;
using Services;
using System;

namespace PITL.Extract.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.AddSingleton<IPowerService, PowerService>();
                    services.AddSingleton<IAggregator, Aggregator>();
                    services.AddSingleton<IClock, Clock>();
                    services.AddSingleton<IFailSafeSource>(x => // TODO
                       ActivatorUtilities.CreateInstance<FailSafeSource>(x,  DateTime.Now.AddSeconds(1)));
                    services.AddSingleton<IPositionReader, PositionReader>();
                    services.AddSingleton<ITimeStamper, TimeStamper>();
                    services.AddSingleton<IValidator, Validator>();

                    services.AddSingleton<ICsvFileCreator, CsvFileCreator>();
                    services.AddSingleton<ICsvStringBuilder, CsvStringBuilder>();
                    services.AddSingleton<IFileNameGenerator>(x => // TODO
                       ActivatorUtilities.CreateInstance<FileNameGenerator>(x, ""));
                });
    }
}
