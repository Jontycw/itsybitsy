using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ItsyBitsy.Domain
{
    public interface IDownloader
    {
        public Task<DownloadResult> DownloadAsync(string uri);
        void Dispose();
    }


    public class Downloader : CrawlWorkerBase, IDisposable
    {
        private bool disposed = false;
        private readonly bool _followExternalLinks = false;
        private readonly bool _downloadExternalContent = false;
        private readonly HttpClient _client;
        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly ICrawlProgress _progress;
        private readonly string _seed;

        public Downloader(Uri host, ISettings settings, ICrawlProgress progress, bool separateThread = true)
            : base(separateThread)
        {
            _seed = string.Intern(host.ToString());
            _progress = progress;
            ServicePointManager.DefaultConnectionLimit = 50;
            _semaphoreSlim = new SemaphoreSlim(20);
            var handler = Factory.GetInstance<HttpClientHandler>();
            handler.AllowAutoRedirect = settings.FollowRedirects;
            handler.MaxAutomaticRedirections = 10;
            handler.AutomaticDecompression = DecompressionMethods.All;
            handler.CookieContainer = new CookieContainer();
            handler.MaxConnectionsPerServer = 15;
            handler.UseCookies = settings.UseCookies;

            _followExternalLinks = settings.FollowExtenalLinks;
            _downloadExternalContent = settings.DownloadExternalContent;

            _client = new HttpClient(handler)
            {
                BaseAddress = host
            };
            _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "ItsyBitsy");
        }

        /// <summary>
        /// Downloads web content from a given uri.
        /// </summary>
        /// <param name="uri">The url to download</param>
        /// <returns>html content</returns>
        protected override void DoWorkInternal()
        {
            try
            {
                if (!Crawler.DownloadQueue.TryTake(out ParentLink nextLink, 1000))
                {
                    //if (_progress.TotalLinkCount == _progress.LinksAcknowledged
                    //    && _semaphoreSlim.CurrentCount == 20
                    //    && Crawler.DownloadQueue.FirstOrDefault() == null
                    //    && Crawler.NewLinks.FirstOrDefault() == null
                    //    && Crawler.DownloadResults.FirstOrDefault() == null)
                    //{
                    //    Crawler.NewLinks.CompleteAdding();
                    //    Crawler.DownloadQueue.CompleteAdding();
                    //    Crawler.DownloadResults.CompleteAdding();
                    //}

                    return;
                }

                _semaphoreSlim.Wait();

                if (_separateThread)
                {
                    Task.Run(async () =>
                    {
                        await DownloadLinkAsync(nextLink);
                    })
                    .ContinueWith((x) =>
                    {
                        _progress.TotalDownloadResult++;
                        _semaphoreSlim.Release();
                    });
                }
                else
                {
                    DownloadLinkAsync(nextLink).Wait();
                    _progress.TotalDownloadResult++;
                    _semaphoreSlim.Release();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task DownloadLinkAsync(ParentLink nextLink)
        {
            DownloadResult result = new DownloadResult(nextLink);

            bool isDomainResource = result.Uri.StartsWith(_seed);
            bool shouldAddLink(ContentType x) => (x == ContentType.Html && (_followExternalLinks || isDomainResource))
                    || (x != ContentType.Html && (_downloadExternalContent || isDomainResource));

            try
            {
                Stopwatch watch = new Stopwatch();

                watch.Start();
                var getResult = await _client.GetAsync(result.Uri);
                watch.Stop();
                result.Status = ((int)getResult.StatusCode).ToString();

                if (getResult.IsSuccessStatusCode)
                {
                    result.IsSuccessCode = true;
                    var resultContent = getResult.Content;
                    result.ContentType = GetContentType(resultContent.Headers.ContentType?.MediaType);

                    if (shouldAddLink(result.ContentType))
                    {
                        var bytes = await resultContent.ReadAsByteArrayAsync();
                        result.ContentLengthBytes = bytes.Length;
                        result.Content = Encoding.ASCII.GetString(bytes);
                        result.DownloadTime = watch.ElapsedMilliseconds;
                        result.Redirectedto = getResult.RequestMessage.RequestUri.AbsoluteUri;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                result.Exception = e;
                Console.WriteLine($"ERROR: {nextLink.Link}, {e.Message}");
            }
            finally
            {
                if (shouldAddLink(result.ContentType))
                {
                    _progress.Add(result.ToViewdownloadResult());
                    Crawler.DownloadResults.Add(result);
                }
            }
        }

        protected override bool TerminateCondition() => _progress.TotalLinks > 1 && _progress.TotalLinks == _progress.TotalDiscarded + _progress.TotalDownloadResult;

        private ContentType GetContentType(string mediaType)
        {
            if (string.IsNullOrWhiteSpace(mediaType))
                return ContentType.Other;

            switch (mediaType)
            {
                case "text/html":
                case "application/xhtml+xml":
                    return ContentType.Html;
                case "application/javascript":
                case "application/x-javascript":
                case "text/javascript":
                    return ContentType.Javascript;
                case "text/css": return ContentType.Css;
                case "image/jpeg":
                case "image/png":
                case "image/gif":
                case "image/x-icon":
                case "image/svg+xml":
                case "image/svg":
                case "image/vnd.microsoft.icon":
                    return ContentType.Image;
                case "application/json":
                    return ContentType.Json;
                case "application/pdf":
                case "text/xml":
                case "application/font-woff":
                case "application/zip":
                    return ContentType.Other;
                default: throw new Exception($"Unknown media type {mediaType}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                _client.Dispose();

            disposed = true;
        }

        ~Downloader()
        {
            Dispose(false);
        }
    }
}
