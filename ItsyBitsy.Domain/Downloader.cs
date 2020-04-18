using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ItsyBitsy.Data;

namespace ItsyBitsy.Domain
{
    public interface IDownloader
    {
        public Task<DownloadResult> DownloadAsync(string uri);
        void Dispose();
    }

    public class Downloader : IDownloader, IDisposable
    {
        private bool disposed = false;
        private readonly HttpClient _client;

        public Downloader(Uri host)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10,
                AutomaticDecompression = System.Net.DecompressionMethods.All,
                CookieContainer = new System.Net.CookieContainer(),
                MaxConnectionsPerServer = 15,
                UseCookies = false
            };

            _client = new HttpClient(handler);
            _client.BaseAddress = host;
        }

        /// <summary>
        /// Downloads web content from a given uri.
        /// </summary>
        /// <param name="uri">The url to download</param>
        /// <returns>html content</returns>
        public async Task<DownloadResult> DownloadAsync(string uri)
        {
            DownloadResult result = new DownloadResult(uri);

            try
            {
                var getResult = await _client.GetAsync(uri);
                result.Status = getResult.StatusCode.ToString();

                if(getResult.IsSuccessStatusCode)
                {
                    var resultContent = getResult.Content;
                    Stopwatch watch = new Stopwatch();
                    result.ContentType = GetContentType(resultContent.Headers.ContentType.MediaType);

                    watch.Start();
                    result.Content = await resultContent.ReadAsStringAsync();
                    watch.Stop();
                    result.DownloadTime = watch.ElapsedMilliseconds;
                }
            }
            catch(HttpRequestException e)
            {
                result.Exception = e;
                Console.WriteLine($"ERROR: {uri}, {e.Message}");
            }

            return result;
        }

        private ContentType GetContentType(string mediaType)
        {
            switch(mediaType)
            {
                case "text/html": return ContentType.Html;
                case "text/javascript": return ContentType.Javascript;
                case "text/css": return ContentType.Css;
                case "text/jpeg": return ContentType.Image;
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
