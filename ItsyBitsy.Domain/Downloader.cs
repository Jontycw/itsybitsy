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
                UseCookies = false,
            };

            _client = new HttpClient(handler);
            _client.BaseAddress = host;
            //_client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36");
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
                Stopwatch watch = new Stopwatch();
                watch.Start();
                var getResult = await _client.GetAsync(uri);
                watch.Stop();
                result.Status = ((int)getResult.StatusCode).ToString();

                if (getResult.IsSuccessStatusCode)
                {
                    result.IsSuccessCode = true;
                    var resultContent = getResult.Content;
                    result.ContentType = GetContentType(resultContent.Headers.ContentType?.MediaType);

                    var bytes = await resultContent.ReadAsByteArrayAsync();
                    result.ContentLengthBytes = bytes.Length;
                    result.Content = BitConverter.ToString(bytes);
                    result.DownloadTime = watch.ElapsedMilliseconds;
                }
            }
            catch (HttpRequestException e)
            {
                result.Exception = e;
                Console.WriteLine($"ERROR: {uri}, {e.Message}");
            }

            return result;
        }

        private ContentType GetContentType(string mediaType)
        {
            if (string.IsNullOrWhiteSpace(mediaType))
                return ContentType.Other;

            switch(mediaType)
            {
                case "text/html": return ContentType.Html;
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
