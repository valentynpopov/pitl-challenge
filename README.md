# PITL Coding Challenge

## Platform
The solution requires .NET 5 to be built.

It can be executed as a console from Visual Studio 2019, or installed as a Windows Service using sc.exe.
To prepare binaries for Windows Service, WindowsService.pubxml profile can be used after clicking Publish.

## Approach
The aim was to create a minimum viable product, and hopefully avoid major mistakes.

Assessing coding exercises is time-consuming enough even without candidates throwing in unnessary libraries, 
performing premature optimisation, using complex design patterns and implementing features they were not asked to implement.

The detailed list of potential improvements is at the end of this document.

## Naming
Several classes have very short names such as Aggregator, rather than something like DayAheadPowerPositionAggregator.
I believe that is acceptable in such a single-purpose microservice.

## Assumptions
### Durability
As it is required to never miss a scheduled extract, we attempt to create a self-healing system.
It is assumed that in case of a non-critical error we can create an empty extract, to be consumed by the downstream systems.

The idea is that we are not sure whether someone will be monitoring logs constantly, so it might be better to produce 
an empty extract to draw someone''s attention.

However, in case of a critical error we will miss an extract (rather than create it using unrealiable data).

Instead of limit the maximum number of retries when calling PowerService, 
we set a deadline: the final attempt to call PowerService should be made no later than
30 seconds after the first one.

It is assumed that caching the previous PowerService response and returning it in case of a failure
would do no good.

### Time
My understanding of the requirements is that it is OK to treat GMT as local time, and to assume 
that PowerService returns GMT too.

For the purpose of the this challenge it is assumed GMT is an equivalent of UTC.

It is assumed that our scheduling requirements are not very strict, and we do not have to worry, for example, about PC clock being adjusted.
Therefore, it is OK to use Task.Delay() rather than kick off a job at an exact specific time.

### PowerService
As per specification, we do not allow customising the number of periods - it is always 24.

It is been noted that the service accept date and time, and returns the same date and time as supplied. However,
my understanding is we should passing just the date portion.

It is acceptable for the service not to return any trades. A null response will be suspisious, but would also be acceptable.

Although the service always return 24 periods for a trade, it will be acceptable for it to miss certain periods 
(that will be treated as zero volumes). 

It is also acceptable for the service to return non-standard periods less than 1 or more than 24, 
although those would be ignored.

Periods do not have to ordered in the ascending order.

It is not acceptable for the service to return duplicate periods for the same trade, as it is not clear how to aggregate them.
It is also not acceptable if the date returned by the service does not match the requested date (although we will ignore
differences in the time portion).

## Potential improvements for productionisation
### Features
I would consider collecting statistics about the average response time of PowerService, 
so that we could automatically adjust the maximum duration during which we could keep retrying calls to the service.

Also, we may want to implement a PowerService timeout, i.e. create an empty extract if it is taking too long for the service
to respond.

It might be useful to create extracts retrospectively, i.e. check if we have any missing extracts and create them during the service start-up.
However, from the PowerService API it is not clear whether it will return accurate results for a specific time of the day.

Configuration is not loaded dynamically, although that is a normal practice for a Windows Service.

In addition to logging, it could be useful to support Performance Counters and a telemetry system, such as Prometheus.

We might want to be able to configure the file name pattern, not just the output path.

### Coding style
I would have considered using Chain of Resposibility pattern for processing PowerTrade objects.

It might be better not to hardcode "24" as the number of periods.

If .NET 6 was publicly available, I would have used DateOnly and TimeOnly classes.

### Optimisation
It was assumed that time spent processing trades is always neglibile comparing to the slow PowerService.
If it was not the case, then we could, for example, stop using Linq for aggregation.
Also, instead of having a record with Period and Volume we could just have an array with 24 elements 
where array index corresponds to the period.

### Third-party libraries
For a commerical project I might have considered using the following libraries:
- Specflow
- Topshelf for bootstrapping Windows Service
- CsvHelper
- Quartz.Net or Hangfire for scheduling jobs
- Polly for retries and circuit breaker
- NodaTime
- TPL Dataflow
- NInject for convention-based DI and better factory support

