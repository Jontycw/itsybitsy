using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ItsyBitsy.Crawler
{
    public class CrawlProgress : ICrawlProgress
    {
        int _totalInQueue;
        public int TotalLinks
        {
            get { return _totalInQueue; }
            set
            {
                Interlocked.Increment(ref _totalInQueue);
            }
        }
        public int TotalDiscarded { get; set; }
        public int TotalDownloadResult { get; set; }
        public string StatusText => TotalLinks > 0 ? $"{TotalDiscarded}/{TotalLinks} {((TotalDiscarded * 1.0) / TotalLinks * 100.0):0.##}% Errors:{TotalDiscarded - TotalDownloadResult}" : string.Empty;

        public void Add(ContentType contentType)
        {
            ContentTypeDistribution[contentType]++;
        }

        public void Add(DownloadResult downloadResult)
        {
            throw new NotImplementedException();
        }

        private Dictionary<ContentType, int> ContentTypeDistribution { get; } = new Dictionary<ContentType, int>()
        {
            { ContentType.Css, 0 },
            { ContentType.Html, 0 },
            { ContentType.Image, 0 },
            { ContentType.Javascript, 0 },
            { ContentType.Json, 0 },
            { ContentType.Other, 0 }
        };
    }
}
