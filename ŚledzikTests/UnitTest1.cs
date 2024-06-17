using System;
using System.Diagnostics;
using System.Security;
using NUnit.Framework;
using System.Security.Principal;
using NUnit.Framework.Internal.Execution;
using static SledzikLibrary.ServiceLibrary;

namespace ŚledzikTests
{

    [TestFixture]
    public class ServiceTests
    {
        [Test]
        public void CreatingPerfoCounterThrowsExceptionWhenNoInstance()
        {
            string instance = "SledzikTestowy#242";
            string counterName = "% Processor Time";

            Assert.Throws<InvalidOperationException>(() => CreatePerformanceCounter(instance, counterName));
        }


        [Test]
        public void LogMessageIsReturningCorrectString()
        {
            string trackedApp = "trackedApp";
            int length = 1;
            string cpu = "10";
            string ram = "1000";
            string expectedValue = "trackedApp\n" +
                                   "1\n" +
                                   "10\n" +
                                   "1000\n";

            string result = LogMessage(trackedApp, length, cpu, ram);

            Assert.That(result, Is.EqualTo(expectedValue));
        }
    }
}
