using ItsyBitsy.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ItsyBitsy.Domain
{
    public interface ICrawler
    {
        void Start(Website website, int sessionId);
    }

    public interface ICrawlWorker
    {
        void Start();
        void Stop();
        void Pause();
        void Resume();
    }

    public interface ICrawlProgress
    {
        int TotalInQueue { get; set; }
        int TotalCrawled { get; set; }
        int TotalSuccess { get; set; }
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
        private CancellationTokenSource _tokenSource;
        private bool _addNewLinks = true;
        private readonly PauseTokenSource _pauseToken;
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
            _pauseToken = new PauseTokenSource();
            _settings = settings;
            _progress = progress;
        }

        public void Start(Website website, int sessionId)
        {
            _sessionId = sessionId;
            _crawlWorkers = new CrawlWorkerBase[3] 
            {
                new Feeder(website.Id, sessionId, _progress, _addNewLinks),
                new Downloader(website.Seed, _settings, _progress),
                new Processor(website, sessionId, _settings, _progress), 
            };

            for (int i = 0; i < _crawlWorkers.Length; i++)
                _crawlWorkers[i].Start();

            _progress.TotalInQueue++;
            DownloadQueue.Add(new ParentLink(website.Seed.ToString(), null));
        }

        public async Task Pause()
        {
            await _pauseToken.PauseAsync();
        }

        public async Task Resume()
        {
            await _pauseToken.ResumeAsync();
        }

        public async Task HardStop()
        {
            _tokenSource.Cancel();
            await Repository.EndSession(_sessionId);
        }
        public void DrainStop()
        {
            _addNewLinks = false;
        }
    }
}
