using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ItsyBitsy.UnitTest
{
    public class FeederTest
    {
        private static List<string> StringList1 = new List<string>() { "http://5" };
        private static List<string> StringList4 = new List<string>() { "http://1", "http://2", "http://3", "http://4" };

        [Fact]
        public async Task DontReprocessTheSameLink()
        {
            var feeder = new Feeder(10);
            await feeder.AddLinks(StringList4, 100, 100, 100);
            Assert.Equal("http://1", feeder.GetNextLink().Link);
            Assert.Equal("http://2", feeder.GetNextLink().Link);
            await feeder.AddLinks(StringList4, 100, 100, 100);
            Assert.Equal("http://3", feeder.GetNextLink().Link);
            Assert.Equal("http://4", feeder.GetNextLink().Link);
            Assert.False(feeder.HasLinks());
        }

        [Fact]
        public async Task KeeplinksInOrder()
        {
            var feeder = new Feeder(10);
            await feeder.AddLinks(StringList4, 100, 100, 100);
            Assert.Equal("http://1", feeder.GetNextLink().Link);
            Assert.Equal("http://2", feeder.GetNextLink().Link);
            await feeder.AddLinks(StringList1, 100, 100, 100);
            Assert.Equal("http://3", feeder.GetNextLink().Link);
            Assert.Equal("http://4", feeder.GetNextLink().Link);
            Assert.Equal("http://5", feeder.GetNextLink().Link);
            Assert.False(feeder.HasLinks());
        }

        [Fact]
        public async Task HasLinksTest()
        {
            var feeder = new Feeder(10);
            Assert.False(feeder.HasLinks());
            await feeder.AddLinks(StringList1, 100, 100, 100);
            Assert.True(feeder.HasLinks());
            feeder.GetNextLink();
            Assert.False(feeder.HasLinks());
        }
    }
}
