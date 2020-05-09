using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using ItsyBitsy.Data;

namespace ItsyBitsy.Domain
{
    //public class ParentLink
    //{
    //    public ParentLink(string link, int? parentId)
    //    {
    //        Link = link;
    //        ParentId = parentId;
    //    }
    //    public string Link { get; }
    //    public int? ParentId { get; }

    //    public override int GetHashCode()
    //    {
    //        return Link.GetHashCode();
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj is ParentLink res)
    //            return res.Link == this.Link;

    //        return false;
    //    }
    //}

    public interface IFeeder
    {
        bool HasLinks();
        string GetNextLink();
        Task<int> AddLinks(IEnumerable<string> links, int parentId, int sessionId, int websiteId);
        void AddSeed(string link);
    }

    /// <summary>
    /// Bug: This class will queue the same link multiple times, need to check if link has already been processed, and only allow 
    /// new links to be returned when calling GetNextLink.
    /// </summary>
    public class Feeder : IFeeder, ICrawlWorker
    {
        private HashSet<string> _alreadyCrawled;
        private readonly BlockingCollection<string> _linksFound;
        private readonly BlockingCollection<string> _downloadQueue;
        private readonly int _websiteId;
        private readonly int _sessionId;

        public Feeder(BlockingCollection<string> linksFound, BlockingCollection<string> downloadQueue, int websiteId, int sessionId)
        {
            _websiteId = websiteId;
            _sessionId = sessionId;
            _linksFound = linksFound;
            _downloadQueue = downloadQueue;
            _alreadyCrawled = new HashSet<string>();
        }

        public async void DoWork()
        {
            while(!_linksFound.IsCompleted)
            {
                var newLinkFound = _linksFound.Take();
                var checkDbTask = Repository.PageExists(newLinkFound);
                if (!_alreadyCrawled.Add(newLinkFound) && await checkDbTask)
                    continue;
                else
                {
                    //write to db if unable to add to download queue.
                    int count = 0;
                    while(!_downloadQueue.TryAdd(newLinkFound, 1000))
                    {
                        await Repository.AddToProcessQueue(newLinkFound, _websiteId, _sessionId);

                        if (count++ > 5)
                            throw new Exception("download queue is full");
                    }
                }
            }
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
                bool isHalfempty = _downloadQueue.Count < 500;
                HashSet<string> existingLinks = new HashSet<string>();
                List<ProcessQueue> overflowProcessQueueItems = new List<ProcessQueue>();

                int linksAddedToQueue = 0;
                foreach (var link in links)
                {
                    if (_alreadyCrawled.Add(link))
                    {
                        if (isHalfempty)
                        {
                            overflowProcessQueueItems.Add(new ProcessQueue()
                            {
                                Link = link,
                                SessionId = sessionId,
                                WebsiteId = websiteId,
                            });
                        }
                        else if(!_downloadQueue.TryAdd(link, 10))
                        {
                            overflowProcessQueueItems.Add(new ProcessQueue()
                            {
                                Link = link,
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

                if (_downloadQueue.Count < 500)
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
                if (!_downloadQueue.TryAdd(queueItem.Link, 50))
                    break;
                else
                    successfullyQueued.Add(queueItem);
            }

            await Repository.RemoveQueuedItems(successfullyQueued);
        }

        public void AddSeed(string link)
        {
            _alreadyCrawled.Add(link);
            _downloadQueue.Add(link);
        }

        public void CompleteAdding()
        {
            _downloadQueue.CompleteAdding();
        }

        public bool HasLinks()
        {
            return _downloadQueue.Any();
        }

        public string GetNextLink()
        {
            return _downloadQueue.Take();
        }
    }
}
