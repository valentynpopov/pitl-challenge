using Microsoft.Extensions.Options;
using PITL.Extract.Job.Abstractions;
using PITL.Extract.Job.Abstractions.Output;
using System;
using System.IO;

namespace PITL.Extract.Job.Output
{
    public class FileNameGenerator : IFileNameGenerator
    {
        private readonly string _outputPath;

        public FileNameGenerator(IOptions<ExtractOptions> options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));
            _outputPath = options.Value.OutputPath
                          ?? throw new ArgumentNullException(nameof(options.Value.OutputPath));
        }

        public string GetFileName(DateTime extractDateTime)
        {
            var fileName = $"PowerPosition_{extractDateTime:yyyyMMdd_HHmm}.csv";
            return Path.Combine(_outputPath, fileName);
        }
    }
}
