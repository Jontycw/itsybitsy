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
        public string Download(string uri);
        public Task<string> DownloadAsync(string uri);
        void Dispose();
    }

    public class Downloader : IDownloader, IDisposable
    {
        private bool disposed = false;
        private readonly HttpClient _client;

        public Downloader()
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
        }

        /// <summary>
        /// Downloads web content from a given uri.
        /// </summary>
        /// <param name="uri">The url to download</param>
        /// <returns></returns>
        public string Download(string uri)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = new HtmlDocument();
            doc = web.Load(uri);


            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];

                if (att.Value.Contains("a"))
                {
                    Console.WriteLine(att.Value);
                }
            }

            return "";
        }

        public Task<string> DownloadAsync(string uri)
        {
            throw new NotImplementedException();
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
