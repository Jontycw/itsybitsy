using ItsyBitsy.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace ItsyBitsy.UnitTest
{
    [TestClass]
    public class FeederTest
    {
        [TestMethod]
        public void Feeder_AddToDownloadQueue()
        {
            BlockingCollection<ParentLink> _newLinks = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<ParentLink> _downloadQueue = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<DownloadResult> _downloadResults = new BlockingCollection<DownloadResult>(new ConcurrentQueue<DownloadResult>(), 10);
            var seedParentLink = new ParentLink(Const.SEED, null);

            var feeder = new Feeder(_newLinks, _downloadQueue, 1, 1, new MockProgess());
            feeder.Start();
            _newLinks.Add(seedParentLink);

            Thread.Sleep(40);
            Assert.IsTrue(_newLinks.Count == 0);
            Assert.AreEqual(_downloadQueue.First(), seedParentLink);
            _downloadQueue.Take();
            feeder.Stop();
            Thread.Sleep(1000);
        }

        //bug here, duplicates are getting through.
        //feeder threads from other tests were still running.
        //be sure to stop them at the end of each test.
        [TestMethod]
        public void FeederTest_NoDuplicateLinks()
        {
            BlockingCollection<ParentLink> _newLinks = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<ParentLink> _downloadQueue = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);

            var seedParentLink = new ParentLink(Const.SEED, null);
            var link2ParentLink = new ParentLink(Const.LINK1, 1);
            var link3ParentLink = new ParentLink(Const.LINK2, 1);
            var mockProgrss = new MockProgess();

            var feeder = new Feeder(_newLinks, _downloadQueue, 1, 1, mockProgrss);
            feeder.Start();
            _newLinks.Add(seedParentLink);
            _newLinks.Add(link2ParentLink);
            _newLinks.Add(link3ParentLink);
            _newLinks.Add(link2ParentLink);
            _newLinks.Add(link3ParentLink);
            _newLinks.Add(link3ParentLink);
            _newLinks.Add(link2ParentLink);
            _newLinks.Add(link3ParentLink);
            _newLinks.Add(link2ParentLink);

            Thread.Sleep(100);
            Assert.IsTrue(_newLinks.Count == 0);
            //should only have 2 items, the duplicate shouldn't come through.
            Assert.AreEqual(3, _downloadQueue.Count);

            //the links should come in the order they were added.
            Assert.AreEqual(Const.SEED, _downloadQueue.Take().Link);
            Assert.AreEqual(Const.LINK1, _downloadQueue.Take().Link);
            Assert.AreEqual(Const.LINK2, _downloadQueue.Take().Link);

            feeder.Stop();
            Thread.Sleep(1000);
        }

        [TestMethod]
        public void TestMemoryQueueOverFlow()
        {
            BlockingCollection<ParentLink> _newLinks = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<ParentLink> _downloadQueue = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), 1000);
            BlockingCollection<DownloadResult> _downloadResults = new BlockingCollection<DownloadResult>(new ConcurrentQueue<DownloadResult>(), 10);
            var mockProgrss = new MockProgess();
            var feeder = new Feeder(_newLinks, _downloadQueue, 1, 1, mockProgrss);
            feeder.Start();
            for (int i = 0; i < 1001; i++) //Crawler.MaxCollectionSize + 1
                _newLinks.Add(new ParentLink($"http://{i}.co.za", null));

            //download queue should be at max size of 1000
            Thread.Sleep(1000);
            Assert.AreEqual(1000, _downloadQueue.Count);

            //after dequeuing the item, the database item should be readded taking the total back to 1000
            _downloadQueue.Take();
            Thread.Sleep(2000);
            Assert.AreEqual(1000, _downloadQueue.Count);

            //queue should now have decreased by 1.
            _downloadQueue.Take();
            Thread.Sleep(1000);
            Assert.AreEqual(999, _downloadQueue.Count);
            feeder.Stop();
            Thread.Sleep(1000);
        }

        [TestMethod]
        public void Feeder_Progress()
        {

        }
    }
}