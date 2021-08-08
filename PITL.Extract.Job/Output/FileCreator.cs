using Microsoft.Extensions.Logging;
using PITL.Extract.Job.Abstractions;
using PITL.Extract.Job.Abstractions.Output;
using System;
using System.Collections.Generic;
using System.IO;

namespace PITL.Extract.Job.Output
{
    public class FileCreator : IFileCreator
    {
        private readonly IFileNameGenerator _fileNameGenerator;
        private readonly ICsvStringBuilder _csvStringBuilder;
        private readonly ILogger<FileCreator> _logger;

        public FileCreator(IFileNameGenerator fileNameGenerator, ICsvStringBuilder csvStringBuilder, ILogger<FileCreator> logger)
        {
            _fileNameGenerator = fileNameGenerator;
            _csvStringBuilder = csvStringBuilder;
            _logger = logger;
        }

        public void Create(IReadOnlyList<Position> positions, DateTime extractDateTime)
        {
            var fileName = _fileNameGenerator.GetFileName(extractDateTime);
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
