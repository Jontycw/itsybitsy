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
        IEnumerable<string> GetLinks(string content);
        Task<IEnumerable<string>> GetLinksAsync(string content);
    }

    public class Processor : IProcessor
    {
        private readonly Website _website;
        public Processor(Website website)
        {
            _website = website;
        }

        /// <summary>
        /// Extracts data from an internet response.
        /// </summary>
        /// <param name="responseBody">internet response</param>
        public IEnumerable<string> GetLinks(string content)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href] | //link[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                var pageLink = att.Value;
                var absoluteUri = new Uri(_website.Seed, pageLink).AbsoluteUri;
                yield return absoluteUri;
            }

            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//script[@src] | //img[@src]"))
            {
                HtmlAttribute att = link.Attributes["src"];
                var pageLink = att.Value;
                var absoluteUri = new Uri(_website.Seed, pageLink).AbsoluteUri;
                yield return absoluteUri;
            }
        }

        public Task<IEnumerable<string>> GetLinksAsync(string content)
        {
            throw new NotImplementedException();
        }
    }
}
