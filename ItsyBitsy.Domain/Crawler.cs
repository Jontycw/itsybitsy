using System;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ItsyBitsy.Domain
{
    public interface ICrawler
    {
        void Start(Website website, int sessionId);
    }

    public interface ICrawlWorker
    {
        event EventHandler WorkerComplete;
        void Start();
        void Stop();
        void Pause();
        void Resume();
    }

    public interface ICrawlProgress
    {
        int TotalLinks { get; set; }
        int TotalDiscarded { get; set; }
        int TotalDownloadResult { get; set; }
        string StatusText { get; }
        public void Add(DownloadResult downloadResult);
    }

    public interface ISettings
    {
        bool FollowExtenalLinks { get; set; }
        bool DownloadExternalContent { get; set; }
        //bool RespectRobots { get; set; }
        bool FollowRedirects { get; set; }
        bool UseCookies { get; set; }
        //bool IncludeImages { get; set; }
        //bool IncludeCss { get; set; }
        //bool IncludeJs { get; set; }
        //bool IncludeJson { get; set; }
        //bool IncludeOther { get; set; }
    }

    public class Crawler : ICrawler
    {
        private int _sessionId;
        private readonly CancellationTokenSource _tokenSource;
        private readonly ISettings _settings;
        private readonly ICrawlProgress _progress;

        private const int MaxCollectionSize = 1000;
        public static readonly BlockingCollection<ParentLink> DownloadQueue = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), MaxCollectionSize);
        public static readonly BlockingCollection<ParentLink> NewLinks = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), MaxCollectionSize);
        public static readonly BlockingCollection<DownloadResult> DownloadResults = new BlockingCollection<DownloadResult>(new ConcurrentQueue<DownloadResult>(), 10);

        private ICrawlWorker[] _crawlWorkers;

        public Crawler(ICrawlProgress progress, ISettings settings)
        {
            _tokenSource = new CancellationTokenSource();
            _settings = settings;
            _progress = progress;
        }

        public void Start(Website website, int sessionId)
        {
            _sessionId = sessionId;
            _crawlWorkers = new CrawlWorkerBase[3]
            {
                new Feeder(website.Id, sessionId, _progress),
                new Downloader(website.Seed, _settings, _progress),
                new Processor(website, sessionId, _settings, _progress),
            };

            for (int i = 0; i < _crawlWorkers.Length; i++)
            {
                _crawlWorkers[i].WorkerComplete += Crawler_WorkerComplete;
                _crawlWorkers[i].Start();
            }

            _progress.TotalLinks++;
            DownloadQueue.Add(new ParentLink(website.Seed.ToString(), null));
        }

        bool sessionEnded = false;
        private async void Crawler_WorkerComplete(object sender, EventArgs e)
        {
            if (!sessionEnded)
            {
                sessionEnded = true;
                await Repository.EndSession(_sessionId);
            }
        }

        public void Pause()
        {
            for (int i = 0; i < _crawlWorkers.Length; i++)
                _crawlWorkers[i].Pause();
        }

        public void Resume()
        {
            for (int i = 0; i < _crawlWorkers.Length; i++)
                _crawlWorkers[i].Resume();
        }

        public async Task HardStop()
        {
            _tokenSource.Cancel();
            await Repository.EndSession(_sessionId);
        }
    }
}
