using Microsoft.Extensions.Logging;
using PITL.Extract.Job.Abstractions;
using PITL.Extract.Job.Abstractions.Output;
using System;
using System.Collections.Generic;
using System.IO;

namespace PITL.Extract.Job.Output
{
    public class CsvFileCreator : ICsvFileCreator
    {
        private readonly IFileNameGenerator _fileNameGenerator;
        private readonly ICsvStringBuilder _csvStringBuilder;
        private readonly ILogger<CsvFileCreator> _logger;

        public CsvFileCreator(IFileNameGenerator fileNameGenerator, ICsvStringBuilder csvStringBuilder, ILogger<CsvFileCreator> logger)
        {
            _fileNameGenerator = fileNameGenerator;
            _csvStringBuilder = csvStringBuilder;
            _logger = logger;
        }

        public void Create(DateTime extractTime, IReadOnlyList<Position> positions)
        {
            var fileName = _fileNameGenerator.GetFileName(extractTime);
            var content = _csvStringBuilder.GetContent(positions);

            try
            {
                File.WriteAllText(fileName, content);
            }
            catch(Exception ex)
            {
                _logger.LogError("Could not create {fileName}: {message}", fileName, ex.Message);
            }
        }
    }
}
