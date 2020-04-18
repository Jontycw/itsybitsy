using ItsyBitsy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItsyBitsy.Domain
{
    public interface ICrawler
    {
        Task StartAsync();
    }

    public class Crawler : ICrawler
    {
        private readonly IFeeder _feeder;
        private readonly IProcessor _processor;
        private readonly IDownloader _downloader;
        private readonly Website _website;
        private readonly int _sessionId;

        public Crawler(IFeeder feeder, IProcessor processor, IDownloader downloader, Website website, int sessionId)
        {
            _feeder = feeder;
            _downloader = downloader;
            _processor = processor;
            _website = website;
            _sessionId = sessionId;
        }

        public async Task StartAsync()
        {
            var seed = String.Intern(_website.Seed.ToString());
            while (_feeder.HasLinks())
            {
                var nextLink = _feeder.GetNextLink();
                var downloadResult = await _downloader.DownloadAsync(nextLink.Link);
                var pageId = await Repository.SaveLink(downloadResult, _website.Id, _sessionId, nextLink?.ParentId);
                if (downloadResult.IsSuccessCode && downloadResult.ContentType == ContentType.Html)
                {
                    var newLinks = _processor.GetLinks(downloadResult.Content)
                        .Where(x => x.IsContent || x.Link.StartsWith(seed))
                        .Select(x => x.Link);
                    _feeder.AddLinks(newLinks, pageId);
                }
            }
        }
    }
}
