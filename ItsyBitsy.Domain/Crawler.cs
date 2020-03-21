using System;
using System.Collections.Generic;
using System.Text;

namespace ItsyBitsy.Domain
{
    public class Crawler
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
    }
}
