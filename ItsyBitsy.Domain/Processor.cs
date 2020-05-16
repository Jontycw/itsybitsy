using HtmlAgilityPack;
using System;

namespace ItsyBitsy.Domain
{
    public class Processor : CrawlWorkerBase
    {
        private readonly ISettings _settings;
        private readonly int _sessionId;
        private readonly Website _website;
        private readonly ICrawlProgress _progress;

        public Processor(Website website, int sessionId, ISettings settings, ICrawlProgress progress)
        {
            _settings = settings;
            _website = website;
            _sessionId = sessionId;
            _progress = progress;
        }

        protected override void DoWorkInternal()
        {
            var downloadQueueItem = Crawler.DownloadResults.Take();
            var pageId = Repository.SaveLink(downloadQueueItem, _website.Id, _sessionId);

            if (downloadQueueItem.ContentType != ContentType.Html)
                return;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(downloadQueueItem.Content);
            var docNode = doc.DocumentNode;
            bool foundLinks = false;

            foreach (HtmlNode link in docNode?.SelectNodes("//a[@href] | //link[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                var pageLink = att.Value;
                if (Uri.TryCreate(_website.Seed, pageLink, out Uri absoluteUri) && IsHttpUri(absoluteUri.AbsoluteUri))
                {
                    Crawler.NewLinks.Add(new ParentLink(absoluteUri.AbsoluteUri, pageId));
                    _progress.TotalInQueue++;
                    foundLinks = true;
                }
            }

            foreach (HtmlNode link in docNode?.SelectNodes("//script[@src] | //img[@src]"))
            {
                HtmlAttribute att = link.Attributes["src"];
                var pageLink = att.Value;
                if (Uri.TryCreate(_website.Seed, pageLink, out Uri absoluteUri) && IsHttpUri(absoluteUri.AbsoluteUri))
                {
                    Crawler.NewLinks.Add(new ParentLink(absoluteUri.AbsoluteUri, pageId));
                    _progress.TotalInQueue++;
                    foundLinks = true;
                }
            }

            if (!foundLinks && Crawler.DownloadResults.IsCompleted)
            {
                Crawler.NewLinks.CompleteAdding();
            }
        }

        protected override bool TerminateCondition() => Crawler.DownloadResults.IsCompleted;

        internal static bool IsHttpUri(string uri)
        {
            if (uri == null)
                return false;

            string scheme = new Uri(uri).Scheme;
            return ((string.Compare("http", scheme, StringComparison.OrdinalIgnoreCase) == 0) ||
                (string.Compare("https", scheme, StringComparison.OrdinalIgnoreCase) == 0));
        }
    }
}
