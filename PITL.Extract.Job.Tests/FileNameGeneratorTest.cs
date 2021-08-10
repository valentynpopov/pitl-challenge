using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PITL.Extract.Job.Abstractions;
using PITL.Extract.Job.Output;
using System;

namespace PITL.Extract.Job.Tests
{
    [TestClass]

    public class FileNameGeneratorTest
    {
        private FileNameGenerator _fileNameGenerator;

        [TestInitialize]
        public void Initialize()
        {
            var extractOptions = new ExtractOptions()
            {
                OutputPath = @"\\ReportShare\Power"
            };
            var options = Options.Create(extractOptions);
            _fileNameGenerator = new FileNameGenerator(options);
        }

        [TestMethod]
        public void FileNameGenerated()
        {
            var fileName = _fileNameGenerator.GetFileName(new DateTime(2021, 8, 5, 11, 30, 25));
            Assert.AreEqual(@"\\ReportShare\Power\PowerPosition_20210805_1130.csv", fileName);
        }
    }
}
