using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ItsyBitsy.Domain
{
    public interface IProcessor
    {
        IEnumerable<string> Process(string response);
        Task<IEnumerable<string>> ProcessAsync(string response);
    }

    public class Processor : IProcessor
    {
        public Processor()
        {
        }

        /// <summary>
        /// Extracts data from an internet response.
        /// </summary>
        /// <param name="responseBody">internet response</param>
        public IEnumerable<string> Process(string response)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(response);
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                var pageLink = att.Value;
                Console.WriteLine(pageLink);
                yield return pageLink;
            }
        }

        public Task<IEnumerable<string>> ProcessAsync(string response)
        {
            throw new NotImplementedException();
        }
    }
}
