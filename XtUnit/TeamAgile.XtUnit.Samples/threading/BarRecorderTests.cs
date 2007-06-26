//using System;
//using System.Collections.Generic;
//
//using HK.ATS.Prices;
//using HK.Testing;
//
//using NUnit.Framework;
//
//namespace HK.ATS.Tests
//{
//    [TestFixture]
//    public class BarRecorderTests : Testing.TestCase
//    {
//        private VirtualBroker.Api _virtualBroker;
//
//        public override void BeforeTestCaseRun()
//        {
//            Broker.Api = _virtualBroker = new VirtualBroker.Api(new Log());
//            base.BeforeTestCaseRun();
//        }
//
//        [Test]
//        [ExitTestOnDemand]
//        public void TestRecordingOfBars()
//        {
//            List<TimeSpan> barLengths = new List<TimeSpan>();
//            barLengths.Add(TimeSpan.FromMinutes(5));
//            BarServer barServer = new BarServer(barLengths);
//            Security security = _virtualBroker.CreateSecurity("GBP/USD", MBTrading.Markets.Forex);
//            security.QuoteUpdated += barServer.QuoteUpdated;
//            _virtualBroker.AllQuotesOfAllSecuritiesServed += AllAvailableQuotesServed;
//            _virtualBroker.QuotesServer.DaysToServe = MBTrading.Markets.Forex.HistoricalQuotes.AllAvailableTradingDays;
//
//            BarRecorder barRecorder = new BarRecorder(barServer, security, barLengths[0]);
//
//            security.ReceiveQuotes = true;
//        }
//
//        private void AllAvailableQuotesServed()
//        {
//            TestSucceeded("all available quotes were served");
//        }
//
//    }
//}
