using ItsyBitsy.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace ItsyBitsy.UI
{
    public sealed class CrawlProgress : INotifyPropertyChanged, ICrawlProgress
    {
        private static readonly Lazy<CrawlProgress> lazy = new Lazy<CrawlProgress>(new CrawlProgress());
        public static CrawlProgress Instance { get { return lazy.Value; } }
        public ObservableCollection<DownloadResult> RecentResults { get; } = new ObservableCollection<DownloadResult>();

        private object _lock;

        private CrawlProgress()
        {
            _lock = new object();
            BindingOperations.EnableCollectionSynchronization(RecentResults, _lock);
        }

        int _totalInQueue = 0;
        public int TotalInQueue
        {
            get { return _totalInQueue; }
            set { Interlocked.Increment(ref _totalInQueue); }
        }

        int _totalCrawled = 0;
        public int TotalCrawled
        {
            get { return _totalCrawled; }
            set { Interlocked.Increment(ref _totalCrawled); }
        }

        int _totalSuccess = 0;
        public int TotalSuccess
        {
            get { return _totalSuccess; }
            set { Interlocked.Increment(ref _totalSuccess); }
        }

        public string StatusText => _totalSuccess > 0 ? $"{_totalCrawled}/{_totalInQueue} {((_totalCrawled * 1.0) / _totalInQueue * 100.0):0.##}%" : string.Empty;

        public void Add(DownloadResult downloadResult)
        {
            RecentResults.Insert(0, downloadResult);
            if (RecentResults.Count == 50)
                RecentResults.RemoveAt(49);
            ContentTypeDistribution[downloadResult.ContentType]++;

            NotifyPropertyChanged("TotalInQueue");
            NotifyPropertyChanged("TotalCrawled");
            NotifyPropertyChanged("Statustext");
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
