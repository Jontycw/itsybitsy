using ItsyBitsy.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ItsyBitsy.UnitTest
{
    [TestClass]
    public class TestInit
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Factory.Register<IRepository, MockRepository>();
        }

        [AssemblyCleanup]
        public static void AssemblyClear()
        {
            Factory.Clear();
        }
    }
}
