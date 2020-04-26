using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ItsyBitsy.Domain
{
    public class CrawlManager
    {
        private Crawler _crawler;

        public async Task Start(Website website)
        {
            var feeder = new Feeder();
            var downloader = new Downloader(website.Seed);
            var processor = new Processor(website);
            feeder.AddSeed(website.Seed.ToString());
            var sessionId = await Repository.CreateNewSession();
            _crawler = new Crawler(feeder, processor, downloader, website, sessionId);
            await _crawler.StartAsync();
        }

        public async Task HardStop()
        {
            await _crawler.HardStop();
        }

        public void DrawinStop()
        {
            _crawler.DrainStop();
        }

        public async Task Pause()
        {
            await _crawler.Pause();
        }

        public async Task Resume()
        {
            await _crawler.Resume();
        }
    }
}
