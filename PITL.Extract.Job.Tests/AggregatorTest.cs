using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PITL.Extract.Job.Input;
using Services;
using System;

namespace PITL.Extract.Job.Tests
{
    [TestClass]
    public class AggregatorTest
    {
        private Aggregator _aggregator;

        [TestInitialize]
        public void Initialize()
        {
            var logger = new Mock<ILogger<Aggregator>>();
            _aggregator = new Aggregator(logger.Object);
        }

        [TestMethod]
        public void ThrowsArgumentNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _aggregator.GetAggregatedVolumes(null));
        }

        [TestMethod]
        public void AggregatesTwoTrades()
        {
            // would've been better to use Specflow

            var trade1 = PowerTrade.Create(DateTime.Now, 24);
            for (var period = 1; period <= 24; period++)
                trade1.Periods[period - 1].Volume = 100;

            var trade2 = PowerTrade.Create(DateTime.Now, 24);
            for (var period = 1; period <= 11; period++)
                trade2.Periods[period - 1].Volume = 50;
            for (var period = 12; period <= 24; period++)
                trade2.Periods[period - 1].Volume = -20;

            var aggregated = _aggregator.GetAggregatedVolumes(new[] { trade1, trade2 });

            // have to do it this way because PowerPeriod doesn't redefine Equals()
            for(var period = 1; period <= 24; period++)
            {
                var expected = period <= 11 ? 150 : 80;
                Assert.AreEqual(expected, aggregated[period - 1].Volume);

                Assert.AreEqual(period, aggregated[period - 1].Period);
            }
        }
    }
}
