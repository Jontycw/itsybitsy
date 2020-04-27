﻿using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ItsyBitsy.UI
{
    public sealed class CrawlProgress : INotifyPropertyChanged, ICrawlProgress
    {
        private static readonly Lazy<CrawlProgress> lazy = new Lazy<CrawlProgress>(new CrawlProgress());
        public static CrawlProgress Instance { get { return lazy.Value; } }
        private CrawlProgress() { }

        public int TotalInQueue { get; set; }
        public int TotalCrawled => ContentTypeDistribution.Values.Sum();
        public int TotalSuccess { get; set; }
        public string StatusText => TotalInQueue > 0 ? $"{TotalCrawled}/{TotalInQueue} {((TotalCrawled * 1.0) / TotalInQueue * 100.0):0.##}% Errors:{TotalCrawled - TotalSuccess}" : string.Empty;

        public void Add(ContentType contentType)
        {
            ContentTypeDistribution[contentType]++;
            NotifyPropertyChanged("TotalInQueue");
            NotifyPropertyChanged("TotalCrawled");
            NotifyPropertyChanged("Statustext");
        }

        private  Dictionary<ContentType, int> ContentTypeDistribution { get; } = new Dictionary<ContentType, int>()
        {
            { ContentType.Css, 0 },
            { ContentType.Html, 0 },
            { ContentType.Image, 0 },
            { ContentType.Javascript, 0 },
            { ContentType.Json, 0 },
            { ContentType.Other, 0 }
        };

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
