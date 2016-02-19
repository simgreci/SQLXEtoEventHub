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
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
            scsb.DataSource = "localhost";
            scsb.IntegratedSecurity = true;
            DatabaseContext context = new DatabaseContext(scsb.ConnectionString);

            IStore store = new RegistryStore("second");

            EventConsumer ec = new EventConsumer(context, "C:\\SqlServer\\second", store);

            var events = ec.GetLastEvents();

            System.IO.StringWriter wr = new System.IO.StringWriter();
            JsonSerializer ser = new JsonSerializer();
            ser.Serialize(wr, events[0].HashTable);
            wr.Flush();

            string s = wr.ToString();
        }
    }
}
