using System.Net.Http;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ItsyBitsy.UnitTest")]

namespace ItsyBitsy.Domain
{
    public class DownloadResult
    {
        public DownloadResult(ParentLink parentLink)
        {
            Uri = parentLink.Link;
            ParentId = parentLink.ParentId;
        }

        public int? ParentId { get; }
        public string Uri { get; }
        public string Status { get; internal set; }
        public bool IsSuccessCode { get; internal set; }
        public string Content { get; internal set; }
        public HttpRequestException Exception { get; internal set; }
        public ContentType ContentType { get; internal set; }
        public long DownloadTime { get; internal set; }
        public int ContentLengthBytes { get; internal set; }
        public string Redirectedto { get; internal set; }

        public DownloadResult ToViewdownloadResult()
        {
            return new DownloadResult(new ParentLink(Uri, ParentId))
            {
                Status = Status,
                IsSuccessCode = IsSuccessCode,
                Exception = Exception,
                ContentType = ContentType,
                DownloadTime = DownloadTime,
                ContentLengthBytes = ContentLengthBytes,
                Redirectedto = Redirectedto
            };
        }
    }
}
