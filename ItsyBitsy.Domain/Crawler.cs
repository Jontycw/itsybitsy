using System;
using System.Collections.Generic;
using System.Text;

namespace ItsyBitsy.Domain
{
    public interface ICrawler
    {
        void Start();
    }

    public class Crawler : ICrawler
    {
        private readonly IFeeder _feeder;
        private readonly IProcessor _processor;
        private readonly IDownloader _downloader;

        public Crawler(IFeeder feeder, IProcessor processor, IDownloader downloader)
        {
            _feeder = feeder;
            _downloader = downloader;
            _processor = processor;
        }

        public void Start()
        {
            while (_feeder.HasLinks())
            {
                var nextLink = _feeder.GetNextLink();
                var response = _downloader.Download(nextLink);
                var newLinks = _processor.Process(response);
                _feeder.AddLinks(newLinks);
            }
        }
    }
}
