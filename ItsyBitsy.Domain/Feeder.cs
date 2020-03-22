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
    }

    /// <summary>
    /// Bug: This class will queue the same link multiple times, need to check if link has already been processed, and only allow 
    /// new links to be returned when calling GetNextLink.
    /// </summary>
    public class Feeder : IFeeder
    {
        private readonly BlockingCollection<string> _processQueue;

        public Feeder(int inMemorySize = 10000)
        {
            _processQueue = new BlockingCollection<string>(new ConcurrentQueue<string>(), inMemorySize);
        }

        /// <summary>
        /// Should add to _processQueue if there is space available. (limit 10000 items)
        /// If the queue is full, excess items should be saved to the ProcessQueue databse table.
        /// </summary>
        /// <param name="links"></param>
        public void AddLinks(IEnumerable<string> links)
        {
            foreach (var link in links)
            {
                _processQueue.Add(link);
            }
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
