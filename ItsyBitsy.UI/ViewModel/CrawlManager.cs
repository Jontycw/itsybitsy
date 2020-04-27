using ItsyBitsy.Domain;
using System;
using System.Threading.Tasks;

namespace ItsyBitsy.UI
{
    public sealed class CrawlManager : IDisposable
    {
        private readonly Crawler _crawler;

        public CrawlManager()
        {
            _crawler = new Crawler(CrawlProgress.Instance);
        }

        public async Task Start(Website website)
        {
            var sessionId = await Repository.CreateNewSession();
            await _crawler.StartAsync(website, sessionId);
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

        public void Dispose()
        {
            _crawler.Dispose();
        }

        public class ProgressEventArgs : EventArgs
        {

            public ProgressEventArgs(CrawlProgress progress)
            {
                Progress = progress;
            }

            public CrawlProgress Progress { get; set; }
        }
    }
}
