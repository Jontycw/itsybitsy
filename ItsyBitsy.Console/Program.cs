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
            container.Verify();
        }

        static void Main(string[] args)
        {
            var feeder = container.GetInstance<IFeeder>();
            var downloader = container.GetInstance<IDownloader>();
            var processor = container.GetInstance<IProcessor>();

            while(feeder.HasLinks())
            {
                var nextLink = feeder.GetNextLink();
                var response = downloader.Download(nextLink);
                var newLinks = processor.Process(response);
                feeder.AddLinks(newLinks);
            }
        }
    }
}
