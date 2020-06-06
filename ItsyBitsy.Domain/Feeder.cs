using ItsyBitsy.Data;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;

namespace ItsyBitsy.Domain
{
    public class ParentLink
    {
        public ParentLink(string link, int? parentId)
        {
            Link = link;
            ParentId = parentId;
        }
        public string Link { get; }
        public int? ParentId { get; }

        public override int GetHashCode()
        {
            return Link.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ParentLink res)
                return res.Link == this.Link;

            return false;
        }
    }

    /// <summary>
    /// Feeder take newly found links and makes uncrawled links available for download.
    /// From NewLinks to DownloadQueue
    /// </summary>
    public class Feeder : CrawlWorkerBase
    {
        private readonly HashSet<string> _alreadyCrawled;
        private readonly int _websiteId;
        private readonly int _sessionId;
        private readonly ICrawlProgress _progress;
        private readonly IRepository _repository;

        public Feeder(int websiteId, int sessionId, ICrawlProgress progress, bool separateThread = true)
            : base(separateThread)
        {
            _websiteId = websiteId;
            _sessionId = sessionId;
            _alreadyCrawled = new HashSet<string>();
            _progress = progress;
            _repository = Factory.GetInstance<IRepository>();
        }
        protected override bool TerminateCondition() => _progress.TotalLinks > 1 && _progress.TotalLinks == _progress.TotalDiscarded + _progress.TotalDownloadResult;

        private readonly HashSet<string> _blacklist = new HashSet<string>()
        {
            "https://www.googletagmanager.com",
            "https://www.google.com"
        };

        protected override void DoWorkInternal()
        {
            if (Crawler.NewLinks.TryTake(out ParentLink nextLink, 1000))
            {
                if (!_alreadyCrawled.Add(nextLink.Link) || _repository.PageExists(nextLink.Link, _sessionId)
                    || _blacklist.Any(x => nextLink.Link.StartsWith(x)))
                {
                    _progress.TotalDiscarded++;
                    return;
                }

                if (!Crawler.DownloadQueue.TryAdd(nextLink, 1000))
                    _repository.AddToProcessQueue(nextLink.Link, nextLink.ParentId ?? -1, _websiteId, _sessionId);
            }
            else
            {
                PopulateQueueFromDatabase();
            }
        }

        private void PopulateQueueFromDatabase()
        {
            List<ProcessQueue> successfullyQueued = new List<ProcessQueue>();
            var queueItems = _repository.GetProcessQueueItems(_sessionId, _websiteId);
            foreach (var queueItem in queueItems)
            {
                if (!Crawler.DownloadQueue.TryAdd(new ParentLink(queueItem.Link, queueItem.ParentId), 50))
                    break;
                else
                    successfullyQueued.Add(queueItem);
            }

            _repository.RemoveQueuedItems(successfullyQueued);
        }
    }
}
