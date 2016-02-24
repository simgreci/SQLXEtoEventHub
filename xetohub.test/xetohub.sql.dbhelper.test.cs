using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xetohub.core;
using xetohub.sql;

namespace xetohub.test
{
    [TestClass]
    public class DBHelperTest
    {
        [TestMethod]
        public void XESessionExist()
        {
            using (IDatabaseContext context = new DatabaseContext(Parameters.CONNECTION_STRING))
            {
                Assert.IsTrue(DBHelper.XESessionExist(context, "system_health"));
                Assert.IsFalse(DBHelper.XESessionExist(context, "dummy_trace"));
            }
        }

        [TestMethod]
        public void XEGetSession()
        {
            using (IDatabaseContext context = new DatabaseContext(Parameters.CONNECTION_STRING))
            {
                XESession session = DBHelper.GetSession(context, Parameters.XESESSION_NAME);
                Assert.AreEqual<string>(session.Name, Parameters.XESESSION_NAME);
                Assert.AreEqual<string>(session.FilePath, Parameters.XE_PATH);
            }
        }
    }
}
