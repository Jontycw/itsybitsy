using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ItsyBitsy.Domain
{
    public interface IFeeder
    {
        bool HasLinks();
        string GetNextLink();
        void AddLinks(IEnumerable<string> links);
        void AddLink(string link);
    }

    /// <summary>
    /// Bug: This class will queue the same link multiple times, need to check if link has already been processed, and only allow 
    /// new links to be returned when calling GetNextLink.
    /// </summary>
    public class Feeder : IFeeder
    {
        private readonly BlockingCollection<string> _processQueue;
        private HashSet<string> _alreadyCrawled;

        public Feeder(int inMemorySize = 10000)
        {
            _processQueue = new BlockingCollection<string>(new ConcurrentQueue<string>(), inMemorySize);
            _alreadyCrawled = new HashSet<string>();
        }

        /// <summary>
        /// Should add to _processQueue if there is space available. (limit 10000 items)
        /// If the queue is full, excess items should be saved to the ProcessQueue database table.
        /// </summary>
        /// <param name="links"></param>
        public void AddLinks(IEnumerable<string> links)
        {
            foreach (var link in links)
            {
                if (IsHttpUri(link) && _alreadyCrawled.Add(link))
                {
                    Console.WriteLine(link);
                    _processQueue.Add(link);
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

        public void AddLink(string link)
        {
            _processQueue.Add(link);
        }

        public void CompleteAdding()
        {
            _processQueue.CompleteAdding();
        }

        public bool HasLinks()
        {
            return _processQueue.Any();
        }

        public string GetNextLink()
        {
            return _processQueue.Take();
        }
    }
}
