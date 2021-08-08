using Services;
using System;

namespace PITL.Extract.Job.Abstractions.Input
{
    public interface IFailSafeSource
    {
        PowerTrade[] GetTradesOrEmpty(DateTime date);
    }
}
