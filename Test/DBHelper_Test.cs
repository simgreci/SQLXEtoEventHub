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
            Assert.IsTrue(DBHelper.XESessionExist("system_health"));
            Assert.IsFalse(DBHelper.XESessionExist("dummy_trace"));
        }

        [TestMethod]
        public void XEGetSession()
        {
            XESession session = DBHelper.GetSession("GetEvents");
            Assert.AreEqual<string>(session.Name, "GetEvents");
            Assert.AreEqual<string>(session.FilePath, "C:\\SqlServer\\output");
        }
    }
}
