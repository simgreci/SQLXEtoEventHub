using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLXEtoEventHub;
using SQLXEtoEventHub.XEvent;

namespace Test
{
    [TestClass]
    public class EventHubWriter_Test
    {
        private static string RESULT = "{\r\n\t\"pizza\": \"verace\",\r\n\t\"prezzo\": 14.32,\r\n\t\"quantita\": 3,\r\n\t\"quando\": 1420113600000\r\n}\r\n";

    [TestMethod]
        public void Serialize()
        {
            System.Collections.Generic.Dictionary<string, object> ht = new System.Collections.Generic.Dictionary<string, object>();
            ht["pizza"] = "verace";
            ht["prezzo"] = 14.32;
            ht["quantita"] = 3;
            ht["quando"] = DateTime.Parse("2015-01-01 13:00");

            string s = EventHubWriter.Serialize(ht);

            Assert.AreEqual(s, RESULT);            
        }
    }
}
