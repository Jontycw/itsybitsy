using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ItsyBitsy.UnitTest
{
    public class FeederTest
    {
        private static List<string> StringList1 = new List<string>() { "http://5" };
        private static List<string> StringList4 = new List<string>() { "http://1", "http://2", "http://3", "http://4" };

        [Fact]
        public void DontReprocessTheSameLink()
        {
            var feeder = new Feeder(10);
            feeder.AddLinks(StringList4, null);
            Assert.Equal("http://1", feeder.GetNextLink().Link);
            Assert.Equal("http://2", feeder.GetNextLink().Link);
            feeder.AddLinks(StringList4, null);
            Assert.Equal("http://3", feeder.GetNextLink().Link);
            Assert.Equal("http://4", feeder.GetNextLink().Link);
            Assert.False(feeder.HasLinks());
        }

        [Fact]
        public void KeeplinksInOrder()
        {
            var feeder = new Feeder(10);
            feeder.AddLinks(StringList4, null);
            Assert.Equal("http://1", feeder.GetNextLink().Link);
            Assert.Equal("http://2", feeder.GetNextLink().Link);
            feeder.AddLinks(StringList1, null);
            Assert.Equal("http://3", feeder.GetNextLink().Link);
            Assert.Equal("http://4", feeder.GetNextLink().Link);
            Assert.Equal("http://5", feeder.GetNextLink().Link);
            Assert.False(feeder.HasLinks());
        }

        [Fact]
        public void HasLinksTest()
        {
            var feeder = new Feeder(10);
            Assert.False(feeder.HasLinks());
            feeder.AddLinks(StringList1, null);
            Assert.True(feeder.HasLinks());
            feeder.GetNextLink();
            Assert.False(feeder.HasLinks());
        }
    }
}
