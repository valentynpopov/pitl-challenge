using PITL.Extract.Job.Abstractions.Output;
using System;
using System.IO;

namespace PITL.Extract.Job.Output
{
    public class FileNameGenerator : IFileNameGenerator
    {
        private readonly string _outputPath;

        public FileNameGenerator(string outputPath)
        {
            _outputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));
        }

        public string GetFileName(DateTime extractDateTime)
        {
            var fileName = $"PowerPosition_{extractDateTime:yyyyMMdd_HHmm}.csv";
            return Path.Combine(_outputPath, fileName);
        }
    }
}
