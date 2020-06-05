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

        int _totalLinkCount = 0;
        public int TotalLinkCount
        {
            get { return _totalLinkCount; }
            set { Interlocked.Increment(ref _totalLinkCount); }
        }

        int _linksAcknowledged = 0;
        public int LinksAcknowledged
        {
            get { return _linksAcknowledged; }
            set { Interlocked.Increment(ref _linksAcknowledged); }
        }

        int _totalLinksDownloaded = 0;
        public int TotalLinksDownloaded
        {
            get { return _totalLinksDownloaded; }
            set { Interlocked.Increment(ref _totalLinksDownloaded); }
        }

        public string StatusText => _totalLinksDownloaded > 0 ? $"{_linksAcknowledged}/{_totalLinkCount} {((_linksAcknowledged * 1.0) / _totalLinkCount * 100.0):0.##}%" : string.Empty;

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
