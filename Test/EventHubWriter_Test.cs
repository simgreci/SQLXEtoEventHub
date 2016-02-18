using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLXEtoEventHub;
using SQLXEtoEventHub.XEvent;

namespace Test
{
    [TestClass]
    public class EventHubWriter_Test
    {
        [TestMethod]
        public void Serialize()
        {
            XEPosition x = new XEPosition() { LastFile = "File", Offset = 1000 };
            string s = EventHubWriter.Serialize(x);

            Assert.AreEqual(s, "{\"LastFile\":\"File\",\"Offset\":1000}");            
        }
    }
}
