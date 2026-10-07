using System;
using System.Linq;
using Game.Services.Analytics;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class FakeAnalyticsServiceTests
    {
        private FakeAnalyticsService service;

        [SetUp]
        public void SetUp() => service = new FakeAnalyticsService();

        [Test]
        public void Log_EventWithParameters_IsRecorded()
        {
            service.Log("run_end", ("result", "victory"), ("wave", 12));

            var loggedEvent = service.LoggedEvents.Single();
            Assert.That(loggedEvent.Name, Is.EqualTo("run_end"));
            Assert.That(loggedEvent.Parameters, Is.EqualTo(new (string, object)[] { ("result", "victory"), ("wave", 12) }));
        }

        [Test]
        public void Log_NoParameters_RecordsEmptyParameters()
        {
            service.Log("shop_open");

            Assert.That(service.LoggedEvents.Single().Parameters, Is.Empty);
        }

        [Test]
        public void Log_NullParameters_RecordsEmptyParameters()
        {
            service.Log("shop_open", null);

            Assert.That(service.LoggedEvents.Single().Parameters, Is.Empty);
        }

        [Test]
        public void Log_ParametersMutatedAfterCall_KeepsRecordedValues()
        {
            var parameters = new (string Key, object Value)[] { ("wave", 3) };

            service.Log("run_abandon", parameters);
            parameters[0] = ("wave", 9);

            Assert.That(service.LoggedEvents.Single().Parameters[0].Value, Is.EqualTo(3));
        }

        [Test]
        public void Log_MaxParameters_IsAccepted()
        {
            service.Log("run_end", CreateParameters(FakeAnalyticsService.MaxParameters));

            Assert.That(service.LoggedEvents.Single().Parameters.Count, Is.EqualTo(FakeAnalyticsService.MaxParameters));
        }

        [Test]
        public void Log_TooManyParameters_Throws() =>
            Assert.Throws<ArgumentException>(() => service.Log("run_end", CreateParameters(FakeAnalyticsService.MaxParameters + 1)));

        [TestCase(null)]
        [TestCase("")]
        [TestCase("RunEnd")]
        [TestCase("run-end")]
        [TestCase("_run_end")]
        [TestCase("run__end")]
        public void Log_NonSnakeCaseEventName_Throws(string eventName) =>
            Assert.Throws<ArgumentException>(() => service.Log(eventName));

        [Test]
        public void Log_NonSnakeCaseParameterKey_Throws() =>
            Assert.Throws<ArgumentException>(() => service.Log("run_end", ("Wave", 1)));

        [Test]
        public void ToString_LoggedEvent_ListsParameters()
        {
            service.Log("run_end", ("result", "defeat"), ("wave", 4));

            Assert.That(service.LoggedEvents.Single().ToString(), Is.EqualTo("run_end result=defeat, wave=4"));
        }

        private static (string Key, object Value)[] CreateParameters(int count) =>
            Enumerable.Range(0, count).Select(index => ($"param_{index}", (object)index)).ToArray();
    }
}
