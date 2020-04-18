using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
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

    public interface IFeeder
    {
        bool HasLinks();
        ParentLink GetNextLink();
        void AddLinks(IEnumerable<string> links, int? parentId);
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

        public Feeder(int inMemorySize = 10000)
        {
            _processQueue = new BlockingCollection<ParentLink>(new ConcurrentQueue<ParentLink>(), inMemorySize);
            _alreadyCrawled = new HashSet<ParentLink>();
        }

        /// <summary>
        /// Should add to _processQueue if there is space available. (limit 10000 items)
        /// If the queue is full, excess items should be saved to the ProcessQueue database table.
        /// </summary>
        /// <param name="links"></param>
        public void AddLinks(IEnumerable<string> links, int? parentId)
        {
            foreach (var link in links)
            {
                var newItem = new ParentLink(link, parentId);
                if (IsHttpUri(link) && _alreadyCrawled.Add(newItem))
                {
                    Console.WriteLine(link);
                    _processQueue.Add(newItem);
                }
            }
        }

        internal static bool IsHttpUri(string uri)
        {
            if (uri == null)
                return false;

            string scheme = new Uri(uri).Scheme;
            return ((string.Compare("http", scheme, StringComparison.OrdinalIgnoreCase) == 0) ||
                (string.Compare("https", scheme, StringComparison.OrdinalIgnoreCase) == 0));
        }

        public void AddSeed(string link)
        {
            _processQueue.Add(new ParentLink(link, null));
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
