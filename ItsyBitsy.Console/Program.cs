using ItsyBitsy.Domain;
using System;
using System.Collections.Concurrent;

namespace ItsyBitsy.Crawler
{
    public class Program
    {

        static void Main(string[] args)
        {
            BlockingCollection<string> processQueue = new BlockingCollection<string>(new ConcurrentQueue<string>(), 10000);

            IFeeder feeder = new Feeder(processQueue);
            IDownloader downloader = new Downloader();
            IProcessor processor = new Processor(downloader, processQueue);
        }
    }
}
