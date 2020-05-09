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
        Task StartAsync(Website website, int sessionId);
    }

    public interface ICrawlWorker
    {
        void DoWork();
    }

    public interface ICrawlProgress
    {
        int TotalInQueue { get; set; }
        int TotalCrawled { get; }
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

    public class Crawler : ICrawler, IDisposable
    {
        private readonly IFeeder _feeder;
        private readonly IProcessor _processor;
        private IDownloader _downloader;
        private Website _website;
        private int _sessionId;
        private readonly CancellationTokenSource _tokenSource;
        private bool _addNewLinks = true;
        private readonly PauseTokenSource _pauseToken;
        private readonly ISettings _settings;
        private readonly ICrawlProgress _progress;

        private readonly BlockingCollection<string> _downloadQueue;
        private readonly BlockingCollection<string> _linksFound;
        private readonly BlockingCollection<string> _downloadResults;
        private readonly ICrawlWorker[] _crawlWorkers;

        public Crawler(ICrawlProgress progress, ISettings settings)
        {
            _feeder = new Feeder(_linksFound, _downloadQueue);
            _processor = new Processor(settings);
            _tokenSource = new CancellationTokenSource();
            _pauseToken = new PauseTokenSource();
            _settings = settings;
            _progress = progress;
            _downloadQueue = new BlockingCollection<string>(new ConcurrentQueue<string>(), 1000);
            _downloadResults = new BlockingCollection<string>();
            _linksFound = new BlockingCollection<string>();
            _crawlWorkers = new ICrawlWorker[3];
        }

        public async Task StartAsync(Website website, int sessionId)
        {
            for (int i = 0; i < _crawlWorkers.Length; i++)
            {
                Thread thread = new Thread(_crawlWorkers[i].DoWork);
                thread.IsBackground = true;
                thread.Start();
            }

            _website = website;
            _sessionId = sessionId;
            _downloader = new Downloader(website.Seed, _settings);

            var seed = string.Intern(_website.Seed.ToString());
            var token = _tokenSource.Token;

            _feeder.AddSeed(seed);
            _progress.TotalInQueue++;

            while (!token.IsCancellationRequested && _feeder.HasLinks())
            {
                var nextLink = _feeder.GetNextLink();
                var downloadResult = await _downloader.DownloadAsync(nextLink.Link);
                var pageId = await Repository.SaveLink(downloadResult, _website.Id, _sessionId, nextLink.ParentId);

                if (downloadResult.IsSuccessCode)
                {
                    _progress.TotalSuccess++;
                    if (_addNewLinks && downloadResult.ContentType == ContentType.Html)
                    {
                        var newLinks = _processor.GetLinks(website.Seed, downloadResult.Content)
                            .Where(x => (x.IsContent && (_settings.DownloadExternalContent || x.Link.StartsWith(seed))) || (_settings.FollowExtenalLinks || x.Link.StartsWith(seed)))
                            .Select(x => x.Link);

                        _progress.TotalInQueue += await _feeder.AddLinks(newLinks, pageId, _sessionId, _website.Id);
                    }
                }

                downloadResult.Content = string.Empty;//already processed
                _progress.Add(downloadResult);

                await _pauseToken.PauseIfRequestedAsync(token);
            }
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

        public void Dispose()
        {
            _downloader.Dispose();
        }
    }
}
