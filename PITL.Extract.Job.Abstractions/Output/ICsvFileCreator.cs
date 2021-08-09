using System;
using System.Collections.Generic;

namespace PITL.Extract.Job.Abstractions.Output
{
    public interface ICsvFileCreator
    {
        void Create(DateTime extractTime, IReadOnlyList<Position> positions);
    }
}
