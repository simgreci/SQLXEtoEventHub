using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xetohub.core.store;
using xetohub.core;
using xetohub.core.xevent;

namespace xetohub.test
{
    [TestClass]
    public class SqlStore_Test
    {
        [TestMethod]
        public void Test_SqlStore()
        {
            DatabaseContext context = new DatabaseContext(Parameters.CONNECTION_STRING);

            using (context)
            {
                SqlStore store = new SqlStore(context, "dummy_session");
                XEPosition dummyPositionIn = new XEPosition();
                dummyPositionIn.LastFile = "C:\\dummy_path\\dummy_file.xel";
                dummyPositionIn.Offset = Int64.MaxValue;
                store.Update(dummyPositionIn);
                XEPosition dummyPositionOut = store.Read();
                Assert.AreEqual(dummyPositionIn.LastFile, dummyPositionOut.LastFile);
                Assert.AreEqual(dummyPositionIn.Offset, dummyPositionOut.Offset);
            }
        }
    }
}
