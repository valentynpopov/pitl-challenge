using System;

namespace PITL.Extract.Job.Abstractions.Output
{
    public interface IFileNameGenerator
    {
        string GetFileName(DateTime extractDateTime);
    }
}
