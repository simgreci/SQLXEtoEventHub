using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xetohub.core.eventhub;

namespace xetohub.test
{
    [TestClass]
    public class EventHubPublisher
    {
        private static string GetEnvOrError(string variable)
        {
            var s = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrEmpty(s))
                throw new Exception(string.Format("Must export env variable {0:S} before running this test", variable));

            return s;
        }


        [TestMethod]
        public void PushToEventHub()
        {
            string sbNamespace = GetEnvOrError("EH_NAMESPACE");
            string eventHub = GetEnvOrError("EH_NAME");
            string policy = GetEnvOrError("EH_POLICY");
            string sas = GetEnvOrError("EH_POLICY_KEY");

            string payload = "{ \"color\": \"Red\", \"numero\": 100, \"fagiolo\": 1 }";

            Publisher.PushToEventHub(
                sbNamespace,
                eventHub,
                policy,
                sas,
                TimeSpan.FromDays(1),
                payload);
        }
    }
}
