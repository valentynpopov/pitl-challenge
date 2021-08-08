using Services;
using System;
using System.Collections.Generic;

namespace PITL.Extract.Job.Abstractions.Input
{
    public interface IValidator
    {
        bool IsValidResponse(DateTime extractDateTime, IReadOnlyList<PowerTrade> trades);
    }
}
