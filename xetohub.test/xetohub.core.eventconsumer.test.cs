using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xetohub.core.store;
using xetohub.core;

namespace xetohub.test
{
    [TestClass]
    public class EventConsumer_Test
    {
        [TestMethod]
        public void TestEventConsumer()
        {
            DatabaseContext context = new DatabaseContext(Parameters.CONNECTION_STRING);

            IStore store = new RegistryStore(Parameters.XESESSION_NAME);

            EventConsumer ec = new EventConsumer(context, Parameters.XE_PATH, store);

            var events = ec.GetLastEvents();

            string s = EventHubWriter.Serialize(events[0].Dictionary);
        }
    }
}
