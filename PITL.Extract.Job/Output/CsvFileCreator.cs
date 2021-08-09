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
            _fileNameGenerator = fileNameGenerator ?? throw new ArgumentNullException(nameof(fileNameGenerator));
            _csvStringBuilder = csvStringBuilder ?? throw new ArgumentNullException(nameof(csvStringBuilder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Create(DateTime extractTime, IReadOnlyList<Position> positions)
        {
            var fileName = _fileNameGenerator.GetFileName(extractTime);

            _logger.LogInformation("Generating content for file {fileName}", fileName);
            var content = _csvStringBuilder.GetContent(positions);

            try
            {
                _logger.LogInformation("Creating file {fileName}", fileName);
                File.WriteAllText(fileName, content);
                _logger.LogInformation("Done");
            }
            catch (Exception ex)
            {
                _logger.LogError("Could not create {fileName}: {message}", fileName, ex.Message);
            }
        }
    }
}
