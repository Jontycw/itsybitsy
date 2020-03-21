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

    public class Feeder : IFeeder
    {
        private readonly BlockingCollection<string> _processQueue;

        public Feeder(BlockingCollection<string> processQueue)
        {
            _processQueue = processQueue;
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

        public bool HasLinks()
        {
            return _processQueue.IsCompleted;
        }

        public string GetNextLink()
        {
            return _processQueue.Take();
        }
    }
}
