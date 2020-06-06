using ItsyBitsy.Domain;
using ItsyBitsy.UnitTest.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ItsyBitsy.UnitTest
{
    [TestClass]
    public class ProcessorTest
    {
        [TestMethod]
        public void LinkExtractionTest()
        {
            Assert.IsTrue(Crawler.NewLinks.Count == 0);

            var website = new Website(new Data.Website() { Seed = Const.SEED });
            var processor = new Processor(website, 1, new MockSettings(), new MockProgess(), false);
            Crawler.DownloadResults.Add(new DownloadResult(new ParentLink(Const.LINK1, null))
            {
                Content = MockHtml.Has7Links,
                ContentType = ContentType.Html,
                DownloadTime = 1,
                IsSuccessCode = true,
                Redirectedto = string.Empty,
                Status = "OK"
            });
            processor.Start();

            Assert.AreEqual(7, Crawler.NewLinks.Count);

            while (Crawler.NewLinks.Count > 0)
                Crawler.NewLinks.Take();

            processor.Stop();
        }
    }
}
