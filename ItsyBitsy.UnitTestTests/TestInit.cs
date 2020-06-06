using ItsyBitsy.Domain;
using ItsyBitsy.UnitTest.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;

namespace ItsyBitsy.UnitTest
{
    [TestClass]
    public class TestInit
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Factory.Register<IRepository, MockRepository>();
            Factory.Register<HttpClientHandler, MockHttpClientHandler>();
        }

        [AssemblyCleanup]
        public static void AssemblyClear()
        {
            Factory.Clear();
        }
    }
}
