using System;
using System.Threading;
using System.Threading.Tasks;

namespace ItsyBitsy.Domain
{
    public class CrawlManager : IDisposable
    {
        private readonly Crawler _crawler;

        public CrawlManager()
        {
            _crawler = new Crawler();
        }

        public async Task Start(Website website)
        {
            var sessionId = await Repository.CreateNewSession();
            await _crawler.StartAsync(website, sessionId);
        }

        public CrawlProgressReport CrawlProgressReport => _crawler.CrawlProgressReport;

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

        public void Dispose()
        {
            _crawler.Dispose();
        }

        public class ProgressEventArgs : EventArgs
        {

            public ProgressEventArgs(CrawlProgressReport progress)
            {
                Progress = progress;
            }

            public CrawlProgressReport Progress { get; set; }
        }
    }
}
