using ItsyBitsy.Data;
using System;
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

    public class Crawler : ICrawler, IDisposable
    {
        private readonly IFeeder _feeder;
        private readonly IProcessor _processor;
        private IDownloader _downloader;
        private Website _website;
        private int _sessionId;
        private readonly CancellationTokenSource _tokenSource;
        private bool _addNewLinks = true;
        private PauseTokenSource _pauseToken;

        public CrawlProgressReport CrawlProgressReport { get; } = new CrawlProgressReport();

        public Crawler(IFeeder feeder, IProcessor processor)
        {
            _feeder = feeder;
            _processor = processor;
            _tokenSource = new CancellationTokenSource();
            _pauseToken = new PauseTokenSource();
        }

        public async Task StartAsync(Website website, int sessionId)
        {
            _website = website;
            _sessionId = sessionId;
            _downloader = new Downloader(website.Seed);

            var seed = String.Intern(_website.Seed.ToString());
            var token = _tokenSource.Token;
            while (!token.IsCancellationRequested && _feeder.HasLinks())
            {
                var nextLink = _feeder.GetNextLink();
                var downloadResult = await _downloader.DownloadAsync(nextLink.Link);
                var pageId = await Repository.SaveLink(downloadResult, _website.Id, _sessionId, nextLink.ParentId);

                if (_addNewLinks && downloadResult.IsSuccessCode && downloadResult.ContentType == ContentType.Html)
                {
                    CrawlProgressReport.TotalSuccess++;

                    var newLinks = _processor.GetLinks(website.Seed, downloadResult.Content)
                        .Where(x => x.IsContent || x.Link.StartsWith(seed))
                        .Select(x => x.Link);

                    CrawlProgressReport.TotalInQueue += await _feeder.AddLinks(newLinks, pageId, _sessionId, _website.Id);
                }

                CrawlProgressReport.Add(downloadResult.ContentType);
                await _pauseToken.PauseIfRequestedAsync(token);
            }
        }

        internal async Task Pause()
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
