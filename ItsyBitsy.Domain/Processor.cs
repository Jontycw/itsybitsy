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
        private readonly IRepository _repository;

        public Processor(Website website, int sessionId, ISettings settings, ICrawlProgress progress, bool separateThread = true)
            : base(separateThread)
        {
            _settings = settings;
            _website = website;
            _sessionId = sessionId;
            _progress = progress;
            _repository = Factory.GetInstance<IRepository>();
        }

        protected override void DoWorkInternal()
        {
            if (!Crawler.DownloadResults.TryTake(out DownloadResult downloadQueueItem, 1000))
                return;

            var pageId = _repository.SaveLink(downloadQueueItem, _website.Id, _sessionId);

            if (downloadQueueItem.ContentType != ContentType.Html)
                return;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(downloadQueueItem.Content);
            var docNode = doc.DocumentNode;
            bool foundLinks = false;

            try
            {
                foreach (HtmlNode link in docNode?.SelectNodes("//a[@href] | //link[@href]"))
                {
                    HtmlAttribute att = link.Attributes["href"];
                    var pageLink = att.Value;
                    if (Uri.TryCreate(_website.Seed, pageLink, out Uri absoluteUri) && IsHttpUri(absoluteUri.AbsoluteUri))
                    {
                        Crawler.NewLinks.Add(new ParentLink(absoluteUri.AbsoluteUri, pageId));
                        _progress.TotalLinks++;
                        foundLinks = true;
                    }
                }
            }
            catch { } //empty doc, ignore

            try
            {
                foreach (HtmlNode link in docNode?.SelectNodes("//script[@src] | //img[@src]"))
                {
                    HtmlAttribute att = link.Attributes["src"];
                    var pageLink = att.Value;
                    if (Uri.TryCreate(_website.Seed, pageLink, out Uri absoluteUri) && IsHttpUri(absoluteUri.AbsoluteUri))
                    {
                        Crawler.NewLinks.Add(new ParentLink(absoluteUri.AbsoluteUri, pageId));
                        _progress.TotalLinks++;
                        foundLinks = true;
                    }
                }
            }
            catch { } //empty doc, ignore

            if (!foundLinks && Crawler.DownloadResults.IsCompleted)
            {
                Crawler.NewLinks.CompleteAdding();
            }
        }

        protected override bool TerminateCondition() => _progress.TotalLinks > 1 && _progress.TotalLinks == _progress.TotalDiscarded + _progress.TotalDownloadResult;

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
