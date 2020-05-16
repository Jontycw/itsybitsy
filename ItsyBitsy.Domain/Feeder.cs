using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ItsyBitsy.Data;

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
        private HashSet<string> _alreadyCrawled;
        private readonly int _websiteId;
        private readonly int _sessionId;
        private readonly bool _addNewLinks;
        private readonly ICrawlProgress _progress;

        public Feeder(int websiteId, int sessionId, ICrawlProgress progress, bool addNewLinks)
        {
            _websiteId = websiteId;
            _sessionId = sessionId;
            _alreadyCrawled = new HashSet<string>();
            _progress = progress;
            _addNewLinks = addNewLinks;
        }
        protected override bool TerminateCondition() => Crawler.NewLinks.IsCompleted;

        protected override void DoWorkInternal()
        {
            if (Crawler.NewLinks.TryTake(out ParentLink nextLink, 1000))
            {
                if (!_alreadyCrawled.Add(nextLink.Link) || Repository.PageExists(nextLink.Link, _sessionId))
                {
                    _progress.TotalCrawled++;
                    return;
                }

                if (!Crawler.DownloadQueue.TryAdd(nextLink, 1000))
                    Repository.AddToProcessQueue(nextLink.Link, nextLink.ParentId ?? -1, _websiteId, _sessionId);
            }
            else
            {
                PopulateQueueFromDatabase();
            }
        }

        private void PopulateQueueFromDatabase()
        {
            List<ProcessQueue> successfullyQueued = new List<ProcessQueue>();
            var queueItems = Repository.GetProcessQueueItems(_sessionId, _websiteId);
            foreach (var queueItem in queueItems)
            {
                if (!Crawler.DownloadQueue.TryAdd(new ParentLink(queueItem.Link, queueItem.ParentId), 50))
                    break;
                else
                    successfullyQueued.Add(queueItem);
            }

            Repository.RemoveQueuedItems(successfullyQueued);
        }
    }
}
