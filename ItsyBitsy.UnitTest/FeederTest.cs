using ItsyBitsy.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ItsyBitsy.UnitTest
{
    public class FeederTest
    {
        static FeederTest()
        {
            RegisterUTestFactory.Register();
        }

        private static readonly List<string> StringList1 = new List<string>() { "http://5" };
        private static readonly List<string> StringList4 = new List<string>() { "http://1", "http://2", "http://3", "http://4" };

        [Fact]
        public void Feeder_AddToDownloadQueue()
        {
            Assert.Empty(Crawler.NewLinks);
            Assert.Empty(Crawler.DownloadQueue);

            const string mockSeed = "https://seed.co.za/";
            var seedParentLink = new ParentLink(mockSeed, null);

            var feeder = new Feeder(1, 1, new MockProgess());
            feeder.Start();
            Crawler.NewLinks.Add(seedParentLink);

            Thread.Sleep(40);
            Assert.Empty(Crawler.NewLinks);
            Assert.Single(Crawler.DownloadQueue, seedParentLink);
            Crawler.DownloadQueue.Take();
            feeder.Stop();
            Thread.Sleep(1000);

            Assert.Empty(Crawler.NewLinks);
            Assert.Empty(Crawler.DownloadQueue);
            Assert.Empty(Crawler.DownloadResults);
        }

        //bug here, duplicates are getting through.
        //feeder threads from other tests were still running.
        //be sure to stop them at the end of each test.
        [Fact]
        public void FeederTest_NoDuplicateLinks()
        {
            Assert.Empty(Crawler.NewLinks);
            Assert.Empty(Crawler.DownloadQueue);
            Assert.Empty(Crawler.DownloadResults);

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
            Assert.Empty(Crawler.NewLinks);
            //should only have 2 items, the duplicate shouldn't come through.
            Assert.Equal(3, Crawler.DownloadQueue.Count); 

            //the links should come in the order they were added.
            Assert.Equal(mockSeed, Crawler.DownloadQueue.Take().Link);
            Assert.Equal(link2, Crawler.DownloadQueue.Take().Link);
            Assert.Equal(link3, Crawler.DownloadQueue.Take().Link);

            feeder.Stop();
            Thread.Sleep(1000);

            Assert.Empty(Crawler.NewLinks);
            Assert.Empty(Crawler.DownloadQueue);
        }

        [Fact]
        public void TestMemoryQueueOverFlow()
        {
            Assert.Empty(Crawler.NewLinks);
            Assert.Empty(Crawler.DownloadQueue);
            Assert.Empty(Crawler.DownloadResults);

            var mockProgrss = new MockProgess();
            var feeder = new Feeder(1, 1, mockProgrss);
            feeder.Start();
            for (int i = 0; i < 1001; i++) //Crawler.MaxCollectionSize + 1
                Crawler.NewLinks.Add(new ParentLink($"http://{i}.co.za" , null));

            //download queue should be at max size of 1000
            Thread.Sleep(1000);
            Assert.Equal(1000, Crawler.DownloadQueue.Count);

            //after dequeuing the item, the database item should be readded taking the total back to 1000
            Crawler.DownloadQueue.Take();
            Thread.Sleep(2000);
            Assert.Equal(1000, Crawler.DownloadQueue.Count);

            //queue should now have decreased by 1.
            Crawler.DownloadQueue.Take();
            Thread.Sleep(1000);
            Assert.Equal(999, Crawler.DownloadQueue.Count);
            feeder.Stop();
            Thread.Sleep(1000);

            while (Crawler.DownloadQueue.Count > 0)
                Crawler.DownloadQueue.Take();

            Assert.Empty(Crawler.NewLinks);
            Assert.Empty(Crawler.DownloadQueue);
            Assert.Empty(Crawler.DownloadResults);
        }
    }
}
