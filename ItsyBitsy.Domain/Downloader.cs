using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ItsyBitsy.Domain
{
    public interface IDownloader
    {
        public Task<string> DownloadAsync(string uri);
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
        public async Task<string> DownloadAsync(string uri)
        {
            try
            {
                return await _client.GetStringAsync(uri);
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine($"ERROR: {uri}, {e.Message}");
                return null;
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
