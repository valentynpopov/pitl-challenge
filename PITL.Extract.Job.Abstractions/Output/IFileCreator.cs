using System;
using System.Collections.Generic;

namespace PITL.Extract.Job.Abstractions.Output
{
    public interface IFileCreator
    {
        void Create(IReadOnlyList<Position> positions, DateTime extractDateTime);
    }
}
