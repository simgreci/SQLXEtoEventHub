using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLXEtoEventHubSp;
using System.Data.SqlClient;

namespace Test
{
    [TestClass]
    public class DBHelperTest
    {
        [TestMethod]
        public void XESessionExist()
        {
            using (SqlConnection conn1 = new SqlConnection("Server=localhost;Trusted_Connection=True;"))
            {
                conn1.Open();
                Assert.IsTrue(DBHelper.XESessionExist(conn1, "system_health"));
            }

            using (SqlConnection conn2 = new SqlConnection("Server=localhost;Trusted_Connection=True;"))
            {
                conn2.Open();
                Assert.IsFalse(DBHelper.XESessionExist(conn2, "dummy_trace"));
            }
        }
    }
}
