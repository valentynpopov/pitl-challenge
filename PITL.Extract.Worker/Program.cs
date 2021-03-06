using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PITL.Extract.Job.Abstractions;
using PITL.Extract.Job.Abstractions.Input;
using PITL.Extract.Job.Abstractions.Output;
using PITL.Extract.Job.Input;
using PITL.Extract.Job.Output;
using Serilog;
using Services;

namespace PITL.Extract.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("PITL.Extract_.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();
            
            CreateHostBuilder(args).Build().Run();

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddOptions<ExtractOptions>()
                    .BindConfiguration("ExtractOptions")
                    .ValidateDataAnnotations();

                    services.AddSingleton<IPowerService, PowerService>();
                    services.AddSingleton<IAggregator, Aggregator>();
                    services.AddSingleton<IClock, Clock>();
                    services.AddSingleton<IFailSafeSource, FailSafeSource>();
                    services.AddSingleton<IPositionReader, PositionReader>();
                    services.AddSingleton<ITimeStamper, TimeStamper>();
                    services.AddSingleton<IValidator, Validator>();

                    services.AddSingleton<ICsvFileCreator, CsvFileCreator>();
                    services.AddSingleton<ICsvStringBuilder, CsvStringBuilder>();
                    services.AddSingleton<IFileNameGenerator, FileNameGenerator>();
                })
            .UseSerilog()
            .UseWindowsService();
    }
}
