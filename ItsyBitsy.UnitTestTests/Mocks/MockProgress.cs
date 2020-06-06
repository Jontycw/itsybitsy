using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItsyBitsy.UnitTest
{
    public class MockProgess : ICrawlProgress
    {
        public int TotalLinks { get; set; }
        public int TotalDiscarded { get; set; }
        public int TotalDownloadResult { get; set; }

        public string StatusText => "";

        private List<DownloadResult> result = new List<DownloadResult>();
        public void Add(DownloadResult downloadResult)
        {
            result.Add(downloadResult);
        }
    }
}
