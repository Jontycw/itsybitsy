using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ItsyBitsy.UnitTest
{
    public class FeederTest
    {
        private static List<string> StringList1 = new List<string>() { "5" };
        private static List<string> StringList4 = new List<string>() { "1", "2", "3", "4" };

        [Fact]
        public void DontReprocessTheSameLink()
        {
            var feeder = new Feeder(10);
            feeder.AddLinks(StringList4);
            Assert.Equal("1", feeder.GetNextLink());
            Assert.Equal("2", feeder.GetNextLink());
            feeder.AddLinks(StringList4);
            Assert.Equal("3", feeder.GetNextLink());
            Assert.Equal("4", feeder.GetNextLink());
            Assert.False(feeder.HasLinks());
        }

        [Fact]
        public void KeeplinksInOrder()
        {
            var feeder = new Feeder(10);
            feeder.AddLinks(StringList4);
            Assert.Equal("1", feeder.GetNextLink());
            Assert.Equal("2", feeder.GetNextLink());
            feeder.AddLinks(StringList1);
            Assert.Equal("3", feeder.GetNextLink());
            Assert.Equal("4", feeder.GetNextLink());
            Assert.Equal("5", feeder.GetNextLink());
            Assert.False(feeder.HasLinks());
        }

        [Fact]
        public void HasLinksTest()
        {
            var feeder = new Feeder(10);
            Assert.False(feeder.HasLinks());
            feeder.AddLinks(StringList1);
            Assert.True(feeder.HasLinks());
            feeder.GetNextLink();
            Assert.False(feeder.HasLinks());
        }
    }
}
