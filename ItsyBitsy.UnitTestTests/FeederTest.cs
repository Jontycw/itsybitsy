using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using ItsyBitsy.Domain;
using System.Threading;
using System.Linq;

namespace ItsyBitsy.UnitTest
{
    [TestClass]
    public class FeederTest
    {
        [TestMethod]
        public void Feeder_AddToDownloadQueue()
        {
            Assert.IsTrue(Crawler.NewLinks.Count == 0);
            Assert.IsTrue(Crawler.DownloadQueue.Count == 0);

            const string mockSeed = "https://seed.co.za/";
            var seedParentLink = new ParentLink(mockSeed, null);

            var feeder = new Feeder(1, 1, new MockProgess());
            feeder.Start();
            Crawler.NewLinks.Add(seedParentLink);

            Thread.Sleep(40);
            Assert.IsTrue(Crawler.NewLinks.Count == 0);
            Assert.AreEqual(Crawler.DownloadQueue.First(), seedParentLink);
            Crawler.DownloadQueue.Take();
            feeder.Stop();
            Thread.Sleep(1000);

            Assert.IsTrue(Crawler.NewLinks.Count == 0);
            Assert.IsTrue(Crawler.DownloadQueue.Count == 0);
            Assert.IsTrue(Crawler.DownloadResults.Count == 0);
        }

        //bug here, duplicates are getting through.
        //feeder threads from other tests were still running.
        //be sure to stop them at the end of each test.
        [TestMethod]
        public void FeederTest_NoDuplicateLinks()
        {
            Assert.IsTrue(Crawler.NewLinks.Count == 0);
            Assert.IsTrue(Crawler.DownloadQueue.Count == 0);
            Assert.IsTrue(Crawler.DownloadResults.Count == 0);

            const string mockSeed = "https://seed.co.za/";
            const string link2 = "https://seed.co.za/link2/";
            const string link3 = "https://seed.co.za/link3/";
            var seedParentLink = new ParentLink(mockSeed, null);
            var link2ParentLink = new ParentLink(link2, 1);
            var link3ParentLink = new ParentLink(link3, 1);
            var mockProgrss = new MockProgess();

            var feeder = new Feeder(1, 1, mockProgrss);
            feeder.Start();
            Crawler.NewLinks.Add(seedParentLink);
            Crawler.NewLinks.Add(link2ParentLink);
            Crawler.NewLinks.Add(link3ParentLink);
            Crawler.NewLinks.Add(link2ParentLink);
            Crawler.NewLinks.Add(link3ParentLink);
            Crawler.NewLinks.Add(link3ParentLink);
            Crawler.NewLinks.Add(link2ParentLink);
            Crawler.NewLinks.Add(link3ParentLink);
            Crawler.NewLinks.Add(link2ParentLink);

            Thread.Sleep(100);
            Assert.IsTrue(Crawler.NewLinks.Count == 0);
            //should only have 2 items, the duplicate shouldn't come through.
            Assert.AreEqual(3, Crawler.DownloadQueue.Count);

            //the links should come in the order they were added.
            Assert.AreEqual(mockSeed, Crawler.DownloadQueue.Take().Link);
            Assert.AreEqual(link2, Crawler.DownloadQueue.Take().Link);
            Assert.AreEqual(link3, Crawler.DownloadQueue.Take().Link);

            feeder.Stop();
            Thread.Sleep(1000);

            Assert.IsTrue(Crawler.NewLinks.Count == 0);
            Assert.IsTrue(Crawler.DownloadQueue.Count == 0);
            Assert.IsTrue(Crawler.DownloadResults.Count == 0);
        }

        [TestMethod]
        public void TestMemoryQueueOverFlow()
        {
            Assert.IsTrue(Crawler.NewLinks.Count == 0);
            Assert.IsTrue(Crawler.DownloadQueue.Count == 0);
            Assert.IsTrue(Crawler.DownloadResults.Count == 0);

            var mockProgrss = new MockProgess();
            var feeder = new Feeder(1, 1, mockProgrss);
            feeder.Start();
            for (int i = 0; i < 1001; i++) //Crawler.MaxCollectionSize + 1
                Crawler.NewLinks.Add(new ParentLink($"http://{i}.co.za", null));

            //download queue should be at max size of 1000
            Thread.Sleep(1000);
            Assert.AreEqual(1000, Crawler.DownloadQueue.Count);

            //after dequeuing the item, the database item should be readded taking the total back to 1000
            Crawler.DownloadQueue.Take();
            Thread.Sleep(2000);
            Assert.AreEqual(1000, Crawler.DownloadQueue.Count);

            //queue should now have decreased by 1.
            Crawler.DownloadQueue.Take();
            Thread.Sleep(1000);
            Assert.AreEqual(999, Crawler.DownloadQueue.Count);
            feeder.Stop();
            Thread.Sleep(1000);

            while (Crawler.DownloadQueue.Count > 0)
                Crawler.DownloadQueue.Take();

            Assert.IsTrue(Crawler.NewLinks.Count == 0);
            Assert.IsTrue(Crawler.DownloadQueue.Count == 0);
            Assert.IsTrue(Crawler.DownloadResults.Count == 0);
        }

        [TestMethod]
        public void Feeder_Progress()
        {

        }
    }
}