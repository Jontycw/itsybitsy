using ItsyBitsy.Data;
using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ItsyBitsy.UnitTes
{
    public class MockRepository : IRepository
    {
        private List<ProcessQueue> _processQueues = new List<ProcessQueue>();
        int primaryKey = 0;

        public void AddToProcessQueue(string link, int parentId, int websiteId, int sessionId)
        {
            _processQueues.Add(new ProcessQueue()
            {
                Link = link,
                ParentId = parentId,
                WebsiteId = websiteId,
                SessionId = sessionId,
                Id = ++primaryKey
            });
        }

        public IEnumerable<ProcessQueue> GetProcessQueueItems(int sessionId, int websiteId)
        {
            var nextQueueItems = (from queueItem in _processQueues
                                  where queueItem.SessionId == sessionId && queueItem.WebsiteId == websiteId
                                  orderby queueItem.TimeStamp
                                  select queueItem)
                                 .Take(400);

            return nextQueueItems.ToList();
        }

        public bool PageExists(string newLinkFound, int sessionId)
        {
            return _processQueues.Any(x => x.Link == newLinkFound && x.SessionId == sessionId);
        }

        public void RemoveQueuedItems(IEnumerable<ProcessQueue> successfullyQueued)
        {
            _processQueues.RemoveAll(x => successfullyQueued.Any(y => y.Id == x.Id));
        }

        int pageId = 1;
        public int SaveLink(DownloadResult downloadQueueItem, int id, int sessionId)
        {
            return pageId++;
        }
    }
}
