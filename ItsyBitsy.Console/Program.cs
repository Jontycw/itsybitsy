using ItsyBitsy.Domain;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ItsyBitsy.Crawler
{
    public class Program
    {
        static Program()
        {

        }

        static async Task Main(string[] args)
        {
            var feeder = new Feeder();
            var downloader = new Downloader();
            var processor = new Processor();
            var crawler = new Domain.Crawler(feeder, processor, downloader);
            await crawler.StartAsync();
        }
    }
}
