using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItsyBitsy.UnitTest
{
    public class MockProgess : ICrawlProgress
    {
        public int TotalLinkCount { get; set; }
        public int LinksAcknowledged { get; set; }
        public int TotalLinksDownloaded { get; set; }

        public string StatusText => "";

        private List<DownloadResult> result = new List<DownloadResult>();
        public void Add(DownloadResult downloadResult)
        {
            result.Add(downloadResult);
        }
    }
}
