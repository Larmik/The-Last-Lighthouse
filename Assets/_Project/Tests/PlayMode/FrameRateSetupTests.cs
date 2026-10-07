using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.PlayMode
{
    public sealed class FrameRateSetupTests
    {
        private const int ExpectedTargetFrameRate = 60;

        [Test]
        public void TargetFrameRate_AfterStartup_IsSixty() =>
            Assert.That(Application.targetFrameRate, Is.EqualTo(ExpectedTargetFrameRate));
    }
}
