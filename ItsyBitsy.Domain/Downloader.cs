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

        public Downloader(Uri host, ISettings settings, ICrawlProgress progress)
        {
            _seed = string.Intern(host.ToString());
            _progress = progress;
            ServicePointManager.DefaultConnectionLimit = 50;
            _semaphoreSlim = new SemaphoreSlim(20);
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = settings.FollowRedirects,
                MaxAutomaticRedirections = 10,
                AutomaticDecompression = DecompressionMethods.All,
                CookieContainer = new CookieContainer(),
                MaxConnectionsPerServer = 15,
                UseCookies = settings.UseCookies,
            };

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
                var nextLink = Crawler.DownloadQueue.Take();
                _semaphoreSlim.Wait();

                Task.Run(async () =>
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
                            _progress.TotalSuccess++;
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
                        _progress.TotalCrawled++;
                        if (shouldAddLink(result.ContentType))
                        {
                            _progress.Add(result.ToViewdownloadResult());
                            Crawler.DownloadResults.Add(result);
                        }
                    }

                })
                .ContinueWith((x) => {
                    _semaphoreSlim.Release();

                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override bool TerminateCondition() => Crawler.DownloadQueue.IsCompleted;

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
