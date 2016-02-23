using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLXEtoEventHub;
using System.Data.SqlClient;
using SQLXEtoEventHub.Store;
using Newtonsoft.Json;

namespace Test
{
    [TestClass]
    public class EventConsumer_Test
    {
        [TestMethod]
        public void TestEventConsumer()
        {
            DatabaseContext context = new DatabaseContext(TestParameters.CONNECTION_STRING);

            IStore store = new RegistryStore(TestParameters.XESESSION_NAME);

            EventConsumer ec = new EventConsumer(context, TestParameters.XE_PATH, store);

            var events = ec.GetLastEvents();

            string s = EventHubWriter.Serialize(events[0].Dictionary);
        }
    }
}
