using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Data;

namespace ItsyBitsy.UI
{
    public sealed class CrawlProgress : INotifyPropertyChanged, ICrawlProgress
    {
        private static readonly Lazy<CrawlProgress> lazy = new Lazy<CrawlProgress>(new CrawlProgress());
        public static CrawlProgress Instance { get { return lazy.Value; } }
        public ObservableCollection<DownloadResult> RecentResults { get; } = new ObservableCollection<DownloadResult>();

        private readonly object _lock;

        private CrawlProgress()
        {
            _lock = new object();
            BindingOperations.EnableCollectionSynchronization(RecentResults, _lock);
        }

        int _totalLinks = 0;
        public int TotalLinks
        {
            get { return _totalLinks; }
            set { Interlocked.Increment(ref _totalLinks); }
        }

        int _totalDiscarded = 0;
        public int TotalDiscarded
        {
            get { return _totalDiscarded; }
            set { Interlocked.Increment(ref _totalDiscarded); }
        }

        public int TotalProgress =>  _totalDiscarded + _totalDownloadResult;

        int _totalDownloadResult = 0;
        public int TotalDownloadResult
        {
            get { return _totalDownloadResult; }
            set { Interlocked.Increment(ref _totalDownloadResult); }
        }

        public string StatusText => _totalLinks > 0 ? $"{_totalDiscarded + _totalDownloadResult}/{_totalLinks} {(((_totalDiscarded + _totalDownloadResult) * 1.0) / _totalLinks * 100.0):0.##}%" : string.Empty;

        public void Add(DownloadResult downloadResult)
        {
            RecentResults.Insert(0, downloadResult);
            if (RecentResults.Count == 50)
                RecentResults.RemoveAt(49);
            ContentTypeDistribution[downloadResult.ContentType]++;

            NotifyPropertyChanged("TotalLinks");
            NotifyPropertyChanged("Statustext");
            NotifyPropertyChanged("TotalProgress");
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
