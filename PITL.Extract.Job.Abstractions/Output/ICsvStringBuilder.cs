using System.Collections.Generic;

namespace PITL.Extract.Job.Abstractions.Output
{
    public interface ICsvStringBuilder
    {
        string GetContent(IReadOnlyList<Position> positions);
    }
}
