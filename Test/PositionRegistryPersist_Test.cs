using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLXEtoEventHub;

namespace Test
{
    [TestClass]
    public class PositionRegistryPersist_Test
    {
        [TestMethod]
        public void Create()
        {
            PositionRegistryPersist prp = new PositionRegistryPersist("test");
            prp.Update(new XEPosition { LastFile = "LastFile", Offset = 2000 });
        }

        [TestMethod]
        public void CreateAndLoad()
        {
            PositionRegistryPersist prp = new PositionRegistryPersist("test2");

            var x = new XEPosition { LastFile = "MyLastFile", Offset = 2000 };

            prp.Update(x);

            var x2 = prp.Read();

            Assert.AreEqual(x, x2);
        }
    }
}
