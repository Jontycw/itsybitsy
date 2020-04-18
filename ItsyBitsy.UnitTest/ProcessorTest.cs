using ItsyBitsy.Domain;
using System;
using System.Linq;
using Xunit;

namespace ItsyBitsy.UnitTest
{
    public class ProcessorTest
    {
        [Fact]
        public void LinkExtractionTest()
        {
            var processor = new Processor(new Website(new Data.Website() { Seed = "http://seed" } ));
            var results = processor.GetLinks(MockHtml.Has7Links);
            Assert.Equal(7, results.Count());
        }
    }
}
