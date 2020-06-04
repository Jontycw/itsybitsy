using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItsyBitsy.UnitTest
{
    public class MockProgess : ICrawlProgress
    {
        public int TotalInQueue { get; set; }
        public int TotalCrawled { get; set; }
        public int TotalSuccess { get; set; }

        public string StatusText => "";

        private List<DownloadResult> result = new List<DownloadResult>();
        public void Add(DownloadResult downloadResult)
        {
            result.Add(downloadResult);
        }
    }
}
