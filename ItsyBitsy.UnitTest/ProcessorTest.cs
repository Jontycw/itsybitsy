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
            var processor = new Processor(new Uri("seed"));
            var results = processor.Process(MockHtml.Has5Links);
            Assert.Equal(5, results.Count());
        }
    }
}
