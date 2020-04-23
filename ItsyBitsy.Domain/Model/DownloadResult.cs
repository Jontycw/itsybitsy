using ItsyBitsy.Data;
using System.Net;
using System.Net.Http;

namespace ItsyBitsy.Domain
{
    public class DownloadResult
    {
        public DownloadResult(string uri)
        {
            Uri = uri;
        }

        public string Uri { get; }
        public string Status { get; internal set; }
        public bool IsSuccessCode { get; internal set; }
        public string Content { get; internal set; }
        public HttpRequestException Exception { get; internal set; }
        public ContentType ContentType { get; internal set; }
        public long DownloadTime { get; internal set; }
        public int ContentLengthBytes { get; internal set; }
    }
}
