using ItsyBitsy.Domain;
using System.Collections.Generic;

namespace ItsyBitsy.UnitTest
{
    public class MockProgess : ICrawlProgress
    {
        public int TotalLinks { get; set; }
        public int TotalDiscarded { get; set; }
        public int TotalDownloadResult { get; set; }

        public string StatusText => "";

        private readonly List<DownloadResult> result = new List<DownloadResult>();
        public void Add(DownloadResult downloadResult)
        {
            result.Add(downloadResult);
        }
    }
}
