using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PITL.Extract.Job.Input;
using PITL.Extract.Job.Abstractions.Input;
using Services;
using System;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using PITL.Extract.Job.Abstractions;
using System.Threading;

namespace PITL.Extract.Job.Tests
{
    [TestClass]
    public class FailSafeSourceTest
    {
        private FailSafeSource _failSafeSource;
        private DateTime _extractTime = new DateTime(2021, 8, 5, 10, 1, 1);

        [TestInitialize]
        public void Initialize()
        {
            var powerService = new Mock<IPowerService>();
            powerService.Setup(x => x.GetTrades(It.IsAny<DateTime>()))
                .Throws(new PowerServiceException("Test exception"));

            var clock = new Mock<IClock>();
            clock.Setup(x => x.UtcNow).Returns(_extractTime);

            var extractOptions = new ExtractOptions()
            {
                TimeToGiveUp = TimeSpan.Zero
            };
            var options = Options.Create(extractOptions);

            var logger = new Mock<ILogger<FailSafeSource>>();

            _failSafeSource = new FailSafeSource(powerService.Object, clock.Object, logger.Object, options);
        }

        [TestMethod]
        public void EmptyExractIfException()
        {

            var trades = _failSafeSource.GetTradesOrEmpty(_extractTime, new CancellationToken());
            Assert.AreEqual(0, trades.Length);
        }
    }
}
