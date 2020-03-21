using ItsyBitsy.Domain;
using SimpleInjector;
using System;
using System.Collections.Concurrent;

namespace ItsyBitsy.Crawler
{
    public class Program
    {
        static readonly Container container;
        static Program()
        {
            container = new Container();
            container.Register<IFeeder, Feeder>();
            container.Register<IDownloader, Downloader>();
            container.Register<IProcessor, Processor>();
            container.Register<ICrawler, Domain.Crawler>();
            container.Verify();
        }

        static void Main(string[] args)
        {
            var crawler = container.GetInstance<ICrawler>();
            crawler.Start();
        }
    }
}
