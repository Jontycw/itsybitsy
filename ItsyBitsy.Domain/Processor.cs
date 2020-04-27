using HtmlAgilityPack;
using ItsyBitsy.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ItsyBitsy.Domain
{
    public interface IProcessor
    {
        IEnumerable<PageLink> GetLinks(Uri seed, string content);
        Task<IEnumerable<string>> GetLinksAsync(string content);
    }

    public struct PageLink
    {
        public PageLink(string link, bool isContent)
        {
            IsContent = isContent;
            Link = link;
        }

        public bool IsContent { get; }
        public string Link { get; }
    }

    public class Processor : IProcessor
    {
        private readonly ISettings _settings;
        public Processor(ISettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Extracts data from an internet response.
        /// </summary>
        /// <param name="responseBody">internet response</param>
        public IEnumerable<PageLink> GetLinks(Uri seed, string content)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            var docNode = doc.DocumentNode;

            foreach (HtmlNode link in docNode?.SelectNodes("//a[@href] | //link[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                var pageLink = att.Value;
                if (Uri.TryCreate(seed, pageLink, out Uri absoluteUri) && IsHttpUri(absoluteUri.AbsoluteUri))
                    yield return new PageLink(absoluteUri.AbsoluteUri, link.Name == "link");
            }

            foreach (HtmlNode link in docNode?.SelectNodes("//script[@src] | //img[@src]"))
            {
                HtmlAttribute att = link.Attributes["src"];
                var pageLink = att.Value;
                if (Uri.TryCreate(seed, pageLink, out Uri absoluteUri) && IsHttpUri(absoluteUri.AbsoluteUri))
                    yield return new PageLink(absoluteUri.AbsoluteUri, true);
            }
        }

        internal static bool IsHttpUri(string uri)
        {
            if (uri == null)
                return false;

            string scheme = new Uri(uri).Scheme;
            return ((string.Compare("http", scheme, StringComparison.OrdinalIgnoreCase) == 0) ||
                (string.Compare("https", scheme, StringComparison.OrdinalIgnoreCase) == 0));
        }

        public Task<IEnumerable<string>> GetLinksAsync(string content)
        {
            throw new NotImplementedException();
        }
    }
}
