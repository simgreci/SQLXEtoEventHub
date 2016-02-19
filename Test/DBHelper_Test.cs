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
            using (IDatabaseContext context = new DatabaseContext(TestParameters.CONNECTION_STRING))
            {
                Assert.IsTrue(DBHelper.XESessionExist(context,"system_health"));
                Assert.IsFalse(DBHelper.XESessionExist(context, "dummy_trace"));
            }
        }

        [TestMethod]
        public void XEGetSession()
        {
            using (IDatabaseContext context = new DatabaseContext(TestParameters.CONNECTION_STRING))
            {
                XESession session = DBHelper.GetSession(context, TestParameters.XESESSION_NAME);
                Assert.AreEqual<string>(session.Name, TestParameters.XESESSION_NAME);
                Assert.AreEqual<string>(session.FilePath, TestParameters.XE_PATH);
            }
        }
    }
}
