using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
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

        public Downloader(Uri host, ISettings settings = null)
        {
            ServicePointManager.DefaultConnectionLimit = 50;
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = settings?.FollowRedirects ?? true,
                MaxAutomaticRedirections = 10,
                AutomaticDecompression = DecompressionMethods.All,
                CookieContainer = new CookieContainer(),
                MaxConnectionsPerServer = 15,
                UseCookies = settings?.UseCookies ?? false,
            };

            _client = new HttpClient(handler);
            _client.BaseAddress = host;
            _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "ItsyBitsy");
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
                    result.Content = Encoding.ASCII.GetString(bytes);
                    result.DownloadTime = watch.ElapsedMilliseconds;
                    result.Redirectedto = getResult.RequestMessage.RequestUri.AbsoluteUri;
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
