using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLXEtoEventHubSp;
using SQLXEtoEventHub;

namespace Test
{
    [TestClass]
    public class DBHelperTest
    {
        [TestMethod]
        public void XESessionExist()
        {
            using (IDatabaseContext context = new DatabaseContext("Server = localhost; Trusted_Connection = True;"))
            {
                Assert.IsTrue(DBHelper.XESessionExist(context,"system_health"));
                Assert.IsFalse(DBHelper.XESessionExist(context, "dummy_trace"));
            }
        }

        [TestMethod]
        public void XEGetSession()
        {
            using (IDatabaseContext context = new DatabaseContext("Server = localhost; Trusted_Connection = True;"))
            {
                XESession session = DBHelper.GetSession(context, "GetEvents");
                Assert.AreEqual<string>(session.Name, "GetEvents");
                Assert.AreEqual<string>(session.FilePath, "C:\\SqlServer\\output");
            }
        }
    }
}
