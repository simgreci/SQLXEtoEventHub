using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLXEtoEventHub;
using System.Data.SqlClient;
using SQLXEtoEventHub.Store;

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
            SqlConnection conn = new SqlConnection(scsb.ConnectionString);

            IStore store = new RegistryStore("second");

            EventConsumer ec = new EventConsumer(conn, "C:\\SqlServer\\second", store);

            var events = ec.GetLastEvents();

        }
    }
}
