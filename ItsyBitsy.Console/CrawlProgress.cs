using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItsyBitsy.Crawler
{
    public class CrawlProgress : ICrawlProgress
    {
        public int TotalInQueue { get; set; }
        public int TotalCrawled => ContentTypeDistribution.Values.Sum();
        public int TotalSuccess { get; set; }
        public string StatusText => TotalInQueue > 0 ? $"{TotalCrawled}/{TotalInQueue} {((TotalCrawled * 1.0) / TotalInQueue * 100.0):0.##}% Errors:{TotalCrawled - TotalSuccess}" : string.Empty;

        public void Add(ContentType contentType)
        {
            ContentTypeDistribution[contentType]++;
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
