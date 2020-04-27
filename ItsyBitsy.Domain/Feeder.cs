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

    public interface IFeeder
    {
        bool HasLinks();
        ParentLink GetNextLink();
        Task<int> AddLinks(IEnumerable<string> links, int parentId, int sessionId, int websiteId);
        void AddSeed(string link);
    }

    /// <summary>
    /// Bug: This class will queue the same link multiple times, need to check if link has already been processed, and only allow 
    /// new links to be returned when calling GetNextLink.
    /// </summary>
    public class Feeder : IFeeder
    {
        private readonly BlockingCollection<ParentLink> _processQueue;
        private HashSet<ParentLink> _alreadyCrawled;

        public Feeder(int inMemorySize = 1000)
        {
            _processQueue = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), inMemorySize);
            _alreadyCrawled = new HashSet<ParentLink>();
        }

        /// <summary>
        /// Should add to _processQueue if there is space available. (limit 10000 items)
        /// If the queue is full, excess items should be saved to the ProcessQueue database table.
        /// </summary>
        /// <param name="links"></param>
        public async Task<int> AddLinks(IEnumerable<string> links, int parentId, int sessionId, int websiteId)
        {
            return await Task.Run(async () =>
            {
                bool isHalfempty = _processQueue.Count < 500;
                HashSet<string> existingLinks = new HashSet<string>();
                List<ProcessQueue> overflowProcessQueueItems = new List<ProcessQueue>();

                int linksAddedToQueue = 0;
                foreach (var link in links)
                {
                    var newItem = new ParentLink(link, parentId);
                    if (_alreadyCrawled.Add(newItem))
                    {
                        if (isHalfempty)
                        {
                            overflowProcessQueueItems.Add(new ProcessQueue()
                            {
                                Link = newItem.Link,
                                ParentId = newItem.ParentId.Value,
                                SessionId = sessionId,
                                WebsiteId = websiteId,
                            });
                        }
                        else if(!_processQueue.TryAdd(newItem, 10))
                        {
                            overflowProcessQueueItems.Add(new ProcessQueue()
                            {
                                Link = newItem.Link,
                                ParentId = newItem.ParentId.Value,
                                SessionId = sessionId,
                                WebsiteId = websiteId,
                            });
                        }

                        linksAddedToQueue++;
                    }
                    else
                    {
                        existingLinks.Add(link);
                    }
                }

                if(overflowProcessQueueItems.Any())
                    await Repository.AddToProcessQueue(overflowProcessQueueItems);

                if (_processQueue.Count < 500)
                    await PopulateQueueFromDatabase(sessionId, websiteId);

                await Repository.AddPageRelation(existingLinks, parentId);

                return linksAddedToQueue;
            });
        }

        private async Task PopulateQueueFromDatabase(int sessionId, int websiteId)
        {
            List<ProcessQueue> successfullyQueued = new List<ProcessQueue>();
            var queueItems = Repository.GetProcessQueueItems(sessionId, websiteId);
            foreach (var queueItem in queueItems)
            {
                if (!_processQueue.TryAdd(new ParentLink(queueItem.Link, queueItem.ParentId), 50))
                    break;
                else
                    successfullyQueued.Add(queueItem);
            }

            await Repository.RemoveQueuedItems(successfullyQueued);
        }

        public void AddSeed(string link)
        {
            var parentLink = new ParentLink(link, null);
            _alreadyCrawled.Add(parentLink);
            _processQueue.Add(parentLink);
        }

        public void CompleteAdding()
        {
            _processQueue.CompleteAdding();
        }

        public bool HasLinks()
        {
            return _processQueue.Any();
        }

        public ParentLink GetNextLink()
        {
            return _processQueue.Take();
        }
    }
}
