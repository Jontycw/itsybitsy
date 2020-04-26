using System;
using System.Threading;
using System.Threading.Tasks;

namespace ItsyBitsy.Domain
{
    public class CrawlManager : IDisposable
    {
        private Crawler _crawler;
        private Feeder _feeder;

        //private Timer _timer;

        public CrawlManager()
        {
            _feeder = new Feeder();
            var processor = new Processor();
            _crawler = new Crawler(_feeder, processor);
        }

        public async Task Start(Website website)
        {
            _feeder.AddSeed(website.Seed.ToString());
            var sessionId = await Repository.CreateNewSession();
            await _crawler.StartAsync(website, sessionId);

            //if(OnProgress != null)
            //    _timer = new Timer(state => OnCrawlProgressEvent(new ProgressEventArgs(_crawler.CrawlProgressReport)), null, 1000, 1000);
        }

        //public event EventHandler<ProgressEventArgs> OnProgress;

        //private void OnCrawlProgressEvent(ProgressEventArgs e)
        //{
        //    EventHandler<ProgressEventArgs> handler = OnProgress;

        //    if (handler != null)
        //    {
        //        e.Progress = _crawler.CrawlProgressReport;
        //        handler(this, e);
        //    }
        //}

        public CrawlProgressReport CrawlProgressReport => _crawler.CrawlProgressReport;

        public async Task HardStop()
        {
            await _crawler.HardStop();
            //await _timer.DisposeAsync();
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
            //_timer?.Dispose();
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
