using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xetohub.core.store;
using xetohub.core.xevent;

namespace Test
{
    [TestClass]
    public class RegistryStore_Test
    {
        [TestMethod]
        public void Create()
        {
            RegistryStore prp = new RegistryStore("test");
            prp.Update(new XEPosition { LastFile = "LastFile", Offset = 2000 });
        }

        [TestMethod]
        public void CreateAndLoad()
        {
            RegistryStore prp = new RegistryStore("test2");

            var x = new XEPosition { LastFile = "MyLastFile", Offset = 2000 };

            prp.Update(x);

            var x2 = prp.Read();

            Assert.AreEqual(x, x2);
        }
    }
}