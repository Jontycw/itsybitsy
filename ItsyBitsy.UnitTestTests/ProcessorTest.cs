using ItsyBitsy.Domain;
using ItsyBitsy.UnitTest.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;

namespace ItsyBitsy.UnitTest
{
    [TestClass]
    public class ProcessorTest
    {
        [TestMethod]
        public void LinkExtractionTest()
        {
            BlockingCollection<ParentLink> _newLinks = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<ParentLink> _downloadQueue = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<DownloadResult> _downloadResults = new BlockingCollection<DownloadResult>(new ConcurrentQueue<DownloadResult>(), 10);

            var website = new Website(new Data.Website() { Seed = Const.SEED });
            var processor = new Processor(_downloadResults, _newLinks, website, 1, new MockSettings(), new MockProgess(), false);
            _downloadResults.Add(new DownloadResult(new ParentLink(Const.LINK1, null))
            {
                Content = MockHtml.Has7Links,
                ContentType = ContentType.Html,
                DownloadTime = 1,
                IsSuccessCode = true,
                Redirectedto = string.Empty,
                Status = "OK"
            });
            processor.Start();

            Assert.AreEqual(7, _newLinks.Count);
            processor.Stop();
        }
    }
}
