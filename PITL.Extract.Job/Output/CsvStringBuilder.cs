using PITL.Extract.Job.Abstractions;
using PITL.Extract.Job.Abstractions.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace PITL.Extract.Job.Output
{
    public class CsvStringBuilder : ICsvStringBuilder
    {
        public string GetContent(IReadOnlyList<Position> positions)
        {
            if (positions == null)
                throw new ArgumentNullException(nameof(positions));

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Local Time, Volume");

            foreach (var position in positions)
                stringBuilder.AppendLine($"{position.LocalTime:HH:mm},{position.Volume}");

            return stringBuilder.ToString();
        }
    }
}
