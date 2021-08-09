# pitl-challenge

Short class names

No CsvHelper

Do not implement functionality which was not required, for example
No file name customistion
No dynamic re-loading of config params

Passing date only

Tick Less than a minute? not more than a day?
Not fractions of minutes

Log elapsed time


No backfill of missing extracts

No chain of responsibility

24 hours is hardcoded

linq for aggregation - no premature optimisation

dont care about retries, just time

collect stats about execution time rather than hardcode

add interfaces

Interfaces in the same files

GMT, Utc, BST

It's ok to have zero trades

Performance counters or Telemetry (such as Prometheus)

ignore non-standard periods
                 

TimeOnly, DateOnly

Not using async because fire and forget


Retry forever
Polly
Quartz.Net or Hangfire

NodeTime
No topshelf
Standard DI
Standard log
CsvHelper	

TimeOnly

PowerService:

1-24
can be missing
same date - all trades
same date as requested
it's acceptable not to return any trades
service returns local time - shift daylight saving should take care for

TODO:
output config params

missing period means zero