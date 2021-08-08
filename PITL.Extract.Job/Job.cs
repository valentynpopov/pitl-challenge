using PITL.Extract.Job.Abstractions.Input;
using Services;
using System;

namespace PITL.Extract.Job
{
    public class Job
    {
        private readonly IFailSafeSource _failSafeSource;
        private readonly IAggregator _aggregator;
        private readonly IValidator _validator;

        public Job(IFailSafeSource failSafeSource, IAggregator aggregator, IValidator validator)
        {
            _failSafeSource = failSafeSource;
            _aggregator = aggregator;
            _validator = validator;
        }

        public void Start(DateTime extractDateTime)
        {
            var extractDate = extractDateTime.Date;
            var trades = _failSafeSource.GetTradesOrEmpty(extractDate);
            if (!_validator.IsValidResponse(extractDate, trades))
                trades = Array.Empty<PowerTrade>();
            var aggregated = _aggregator.GetAggregatedVolumes(trades);

        }
    }
}
