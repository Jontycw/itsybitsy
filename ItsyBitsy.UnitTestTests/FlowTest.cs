using ItsyBitsy.Domain;
using ItsyBitsy.UnitTest.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ItsyBitsy.UnitTest
{
    [TestClass]
    public class FlowTest
    {
        [TestMethod]
        public void FlowTest_NoLinks()
        {
            BlockingCollection<ParentLink> _newLinks = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<ParentLink> _downloadQueue = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<DownloadResult> _downloadResults = new BlockingCollection<DownloadResult>(new ConcurrentQueue<DownloadResult>(), 10);

            var progress = new MockProgess();
            var feeder = new Feeder(_newLinks, _downloadQueue, 1, 1, progress);

            var downloader = new Downloader(_downloadQueue, _downloadResults, new Uri(Const.SEED), new MockSettings(), progress, false);
            var processor = new Processor(_downloadResults, _newLinks, new Website(new Data.Website() { Id = 1, Seed = Const.SEED }), 1, new MockSettings(), progress, false);

            //mock behaviour from Crawler.Start
            progress.TotalLinks++;
            _newLinks.Add(new ParentLink(Const.SEED, null));

            Assert.AreEqual(1, progress.TotalLinks);
            feeder.Start();
            Assert.AreEqual(1, progress.TotalLinks);
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(0, _downloadQueue.Count);

            downloader.Start();
            Assert.AreEqual(1, progress.TotalLinks);
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(1, progress.TotalDownloadResult);
            Assert.AreEqual(0, _newLinks.Count);
            Assert.AreEqual(0, _downloadQueue.Count);
            Assert.AreEqual(1, _downloadResults.Count);

            processor.Start();
            Assert.AreEqual(1, progress.TotalLinks);
            Assert.AreEqual(1, progress.TotalDownloadResult);
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(0, _newLinks.Count);
            Assert.AreEqual(0, _downloadQueue.Count);
            Assert.AreEqual(0, _downloadResults.Count);
        }

        [TestMethod]
        public void FlowTest_OneLinkFound()
        {
            BlockingCollection<ParentLink> _newLinks = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<ParentLink> _downloadQueue = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<DownloadResult> _downloadResults = new BlockingCollection<DownloadResult>(new ConcurrentQueue<DownloadResult>(), 10);
            var progress = new MockProgess();
            var feeder = new Feeder(_newLinks, _downloadQueue, 1, 1, progress, false);
            var downloader = new Downloader(_downloadQueue, _downloadResults, new Uri(Const.SEED), new MockSettings(), progress, false);
            var processor = new Processor(_downloadResults, _newLinks, new Website(new Data.Website() { Id = 1, Seed = Const.SEED }), 1, new MockSettings(), progress, false);

            //mock behaviour from Crawler.Start
            _newLinks.Add(new ParentLink(Const.LINK1, 1));
            Assert.AreEqual(0, progress.TotalLinks);
            Assert.AreEqual(1, _newLinks.Count);

            feeder.Start();
            Thread.Sleep(100);
            Assert.AreEqual(0, progress.TotalLinks);
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(0, _newLinks.Count);
            Assert.AreEqual(1, _downloadQueue.Count);
            Assert.AreEqual(0, _downloadResults.Count);

            downloader.Start();
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(0, progress.TotalLinks);
            Assert.AreEqual(1, progress.TotalDownloadResult);
            Assert.AreEqual(0, _newLinks.Count);
            Assert.AreEqual(0, _downloadQueue.Count);
            Assert.AreEqual(1, _downloadResults.Count);

            processor.Start();
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(1, progress.TotalLinks);
            Assert.AreEqual(1, progress.TotalDownloadResult);
            Assert.AreEqual(1, _newLinks.Count);
            Assert.AreEqual(0, _downloadQueue.Count);
            Assert.AreEqual(0, _downloadResults.Count);

            //2nd iteration for new link
            feeder.Start();
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(1, progress.TotalLinks);
            Assert.AreEqual(1, progress.TotalDownloadResult);
            Assert.AreEqual(0, _newLinks.Count);
            Assert.AreEqual(1, _downloadQueue.Count);
            Assert.AreEqual(0, _downloadResults.Count);

            downloader.Start();
            Assert.AreEqual(1, progress.TotalLinks);
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(2, progress.TotalDownloadResult);
            Assert.AreEqual(0, _newLinks.Count);
            Assert.AreEqual(0, _downloadQueue.Count);
            Assert.AreEqual(1, _downloadResults.Count);

            processor.Start();
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(1, progress.TotalLinks);
            Assert.AreEqual(2, progress.TotalDownloadResult);
            Assert.AreEqual(0, _newLinks.Count);
            Assert.AreEqual(0, _downloadQueue.Count);
            Assert.AreEqual(0, _downloadResults.Count);
        }

        [TestMethod]
        public void FlowTest_OneLinkLoopFound()
        {
            BlockingCollection<ParentLink> _newLinks = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<ParentLink> _downloadQueue = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<DownloadResult> _downloadResults = new BlockingCollection<DownloadResult>(new ConcurrentQueue<DownloadResult>(), 10);
            var progress = new MockProgess();
            var feeder = new Feeder(_newLinks, _downloadQueue, 1, 1, progress, false);
            var downloader = new Downloader(_downloadQueue, _downloadResults, new Uri(Const.SEED), new MockSettings(), progress, false);
            var processor = new Processor(_downloadResults, _newLinks, new Website(new Data.Website() { Id = 1, Seed = Const.SEED }), 1, new MockSettings(), progress, false);

            //mock behaviour from Crawler.Start
            _newLinks.Add(new ParentLink(Const.LINK2, 1));
            Assert.AreEqual(0, progress.TotalLinks);
            Assert.AreEqual(1, _newLinks.Count);

            feeder.Start();
            Assert.AreEqual(0, progress.TotalLinks);
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(0, _newLinks.Count);
            Assert.AreEqual(1, _downloadQueue.Count);
            Assert.AreEqual(0, _downloadResults.Count);

            downloader.Start();
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(0, progress.TotalLinks);
            Assert.AreEqual(1, progress.TotalDownloadResult);
            Assert.AreEqual(0, _newLinks.Count);
            Assert.AreEqual(0, _downloadQueue.Count);
            Assert.AreEqual(1, _downloadResults.Count);

            processor.Start();
            Assert.AreEqual(0, progress.TotalDiscarded);
            Assert.AreEqual(1, progress.TotalLinks);
            Assert.AreEqual(1, progress.TotalDownloadResult);
            Assert.AreEqual(1, _newLinks.Count);
            Assert.AreEqual(0, _downloadQueue.Count);
            Assert.AreEqual(0, _downloadResults.Count);

            //2nd iteration for already crawled link, don't crawl duplicates.
            feeder.Start();
            Assert.AreEqual(1, progress.TotalDiscarded);
            Assert.AreEqual(1, progress.TotalLinks);
            Assert.AreEqual(1, progress.TotalDownloadResult);
            Assert.AreEqual(0, _newLinks.Count);
            Assert.AreEqual(0, _downloadQueue.Count);
            Assert.AreEqual(0, _downloadResults.Count);
        }
    }
}
