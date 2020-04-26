using ItsyBitsy.Domain;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ItsyBitsy.Crawler
{
    //todo: Find all paths to a page.
    public class Program
    {
        static async Task Main(string[] args)
        {
            var website = await GetWebsite(args);
            var sessionId = await Repository.CreateNewSession();

            var feeder = new Feeder();
            var downloader = new Downloader(website.Seed);
            var processor = new Processor();
            feeder.AddSeed(website.Seed.ToString());

            var crawler = new Domain.Crawler(feeder, processor);
            await crawler.StartAsync(website, sessionId);
            await Repository.EndSession(sessionId);
            Console.WriteLine("Crawl Finished.");
        }

        private static async Task<Website> GetWebsite(string[] args)
        {
            if (args.Length == 1 && int.TryParse(args[0], out int websiteId))
            {
                var website = await Repository.GetDomainWebsite(websiteId);
                if (website == null)
                {
                    return await GetWebsite(new string[0]);
                }
                else
                {
                    Console.WriteLine($"Starting crawl for {website.Seed}");
                    return website;
                }
            }
            else
            {
                string userWebsiteId;
                int parsedWebsiteId;
                do
                {
                    Console.Write("Please enter a website Id:");
                    userWebsiteId = Console.ReadLine();
                    Console.Clear();

                }
                while (!int.TryParse(userWebsiteId, out parsedWebsiteId));

                var website = await Repository.GetDomainWebsite(parsedWebsiteId);
                if (website == null)
                {
                    return await GetWebsite(new string[0]);
                }
                else
                {
                    Console.WriteLine($"Starting crawl for {website.Seed}");
                    return website;
                }
            }
        }
    }
}
