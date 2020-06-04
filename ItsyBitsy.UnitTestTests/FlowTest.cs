using ItsyBitsy.Domain;
using ItsyBitsy.UnitTest.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ItsyBitsy.UnitTest
{
    [TestClass]
    public class FlowTest
    {
        [TestMethod]
        public void FlowTest_NoLinks()
        {
            var progress = new MockProgess();
            var feeder = new Feeder(1, 1, progress, false);
            var downloader = new Downloader(new Uri(Const.SEED), new MockSettings(), progress, false);
            var processor = new Processor(new Website(new Data.Website() { Id = 1, Seed = Const.SEED }), 1, new MockSettings(), progress, false);

            //mock behaviour from Crawler.Start
            progress.TotalInQueue++;
            Crawler.NewLinks.Add(new ParentLink(Const.SEED, null));

            Assert.AreEqual(1, progress.TotalInQueue);
            feeder.Start();
            Assert.AreEqual(1, progress.TotalInQueue);
            Assert.AreEqual(0, progress.TotalCrawled);
            Assert.AreEqual(1, Crawler.DownloadQueue.Count);

            downloader.Start();
            Assert.AreEqual(1, progress.TotalInQueue);
            Assert.AreEqual(1, progress.TotalCrawled);
            Assert.AreEqual(1, progress.TotalSuccess);
            Assert.AreEqual(0, Crawler.NewLinks.Count);
            Assert.AreEqual(0, Crawler.DownloadQueue.Count);
            Assert.AreEqual(1, Crawler.DownloadResults.Count);

            processor.Start();
            Assert.AreEqual(1, progress.TotalInQueue);
            Assert.AreEqual(1, progress.TotalSuccess);
            Assert.AreEqual(1, progress.TotalCrawled);
            Assert.AreEqual(0, Crawler.NewLinks.Count);
            Assert.AreEqual(0, Crawler.DownloadQueue.Count);
            Assert.AreEqual(0, Crawler.DownloadResults.Count);
        }

        [TestMethod]
        public void FlowTest_OneLinkFound()
        {
            var progress = new MockProgess();
            var feeder = new Feeder(1, 1, progress, false);
            var downloader = new Downloader(new Uri(Const.SEED), new MockSettings(), progress, false);
            var processor = new Processor(new Website(new Data.Website() { Id = 1, Seed = Const.SEED }), 1, new MockSettings(), progress, false);

            //mock behaviour from Crawler.Start
            progress.TotalInQueue++;
            Crawler.NewLinks.Add(new ParentLink(Const.LINK1, 1));
            Assert.AreEqual(1, progress.TotalInQueue);
            Assert.AreEqual(1, Crawler.NewLinks.Count);

            feeder.Start();
            Assert.AreEqual(1, progress.TotalInQueue);
            Assert.AreEqual(0, progress.TotalCrawled);
            Assert.AreEqual(0, Crawler.NewLinks.Count);
            Assert.AreEqual(1, Crawler.DownloadQueue.Count);
            Assert.AreEqual(0, Crawler.DownloadResults.Count);

            downloader.Start();
            Assert.AreEqual(1, progress.TotalInQueue);
            Assert.AreEqual(1, progress.TotalCrawled);
            Assert.AreEqual(1, progress.TotalSuccess);
            Assert.AreEqual(0, Crawler.NewLinks.Count);
            Assert.AreEqual(0, Crawler.DownloadQueue.Count);
            Assert.AreEqual(1, Crawler.DownloadResults.Count);

            processor.Start();
            Assert.AreEqual(2, progress.TotalInQueue);
            Assert.AreEqual(1, progress.TotalSuccess);
            Assert.AreEqual(1, progress.TotalCrawled);
            Assert.AreEqual(1, Crawler.NewLinks.Count);
            Assert.AreEqual(0, Crawler.DownloadQueue.Count);
            Assert.AreEqual(0, Crawler.DownloadResults.Count);

            //2nd iteration for new link
            feeder.Start();
            Assert.AreEqual(2, progress.TotalInQueue);
            Assert.AreEqual(1, progress.TotalSuccess);
            Assert.AreEqual(1, progress.TotalCrawled);
            Assert.AreEqual(0, Crawler.NewLinks.Count);
            Assert.AreEqual(1, Crawler.DownloadQueue.Count);
            Assert.AreEqual(0, Crawler.DownloadResults.Count);

            downloader.Start();
            Assert.AreEqual(2, progress.TotalInQueue);
            Assert.AreEqual(2, progress.TotalCrawled);
            Assert.AreEqual(2, progress.TotalSuccess);
            Assert.AreEqual(0, Crawler.NewLinks.Count);
            Assert.AreEqual(0, Crawler.DownloadQueue.Count);
            Assert.AreEqual(1, Crawler.DownloadResults.Count);

            processor.Start();
            Assert.AreEqual(2, progress.TotalInQueue);
            Assert.AreEqual(2, progress.TotalSuccess);
            Assert.AreEqual(2, progress.TotalCrawled);
            Assert.AreEqual(0, Crawler.NewLinks.Count);
            Assert.AreEqual(0, Crawler.DownloadQueue.Count);
            Assert.AreEqual(0, Crawler.DownloadResults.Count);
        }

        [TestMethod]
        public void FlowTest_OneLinkLoopFound()
        {
            var progress = new MockProgess();
            var feeder = new Feeder(1, 1, progress, false);
            var downloader = new Downloader(new Uri(Const.SEED), new MockSettings(), progress, false);
            var processor = new Processor(new Website(new Data.Website() { Id = 1, Seed = Const.SEED }), 1, new MockSettings(), progress, false);

            //mock behaviour from Crawler.Start
            progress.TotalInQueue++;
            Crawler.NewLinks.Add(new ParentLink(Const.LINK2, 1));
            Assert.AreEqual(1, progress.TotalInQueue);
            Assert.AreEqual(1, Crawler.NewLinks.Count);

            feeder.Start();
            Assert.AreEqual(1, progress.TotalInQueue);
            Assert.AreEqual(0, progress.TotalCrawled);
            Assert.AreEqual(0, Crawler.NewLinks.Count);
            Assert.AreEqual(1, Crawler.DownloadQueue.Count);
            Assert.AreEqual(0, Crawler.DownloadResults.Count);

            downloader.Start();
            Assert.AreEqual(1, progress.TotalInQueue);
            Assert.AreEqual(1, progress.TotalCrawled);
            Assert.AreEqual(1, progress.TotalSuccess);
            Assert.AreEqual(0, Crawler.NewLinks.Count);
            Assert.AreEqual(0, Crawler.DownloadQueue.Count);
            Assert.AreEqual(1, Crawler.DownloadResults.Count);

            processor.Start();
            Assert.AreEqual(2, progress.TotalInQueue);
            Assert.AreEqual(1, progress.TotalSuccess);
            Assert.AreEqual(1, progress.TotalCrawled);
            Assert.AreEqual(1, Crawler.NewLinks.Count);
            Assert.AreEqual(0, Crawler.DownloadQueue.Count);
            Assert.AreEqual(0, Crawler.DownloadResults.Count);

            //2nd iteration for already crawled link, don't crawl duplicates.
            feeder.Start();
            Assert.AreEqual(2, progress.TotalInQueue);
            Assert.AreEqual(1, progress.TotalSuccess);
            Assert.AreEqual(2, progress.TotalCrawled);
            Assert.AreEqual(0, Crawler.NewLinks.Count);
            Assert.AreEqual(0, Crawler.DownloadQueue.Count);
            Assert.AreEqual(0, Crawler.DownloadResults.Count);
        }
    }
}
