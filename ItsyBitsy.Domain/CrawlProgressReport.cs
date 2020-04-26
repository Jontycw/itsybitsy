using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ItsyBitsy.Domain
{
    public class CrawlProgressReport : INotifyPropertyChanged
    {
        public int TotalInQueue { get; set; }
        public int TotalCrawled => ContentTypeDistribution.Values.Sum();
        public int TotalSuccess { get; internal set; }
        public string StatusText => TotalInQueue > 0 ? $"{(TotalCrawled / TotalInQueue * 100):0.##} Errors:{TotalSuccess - TotalCrawled}" : string.Empty;

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

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
