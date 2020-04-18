﻿using ItsyBitsy.Domain;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ItsyBitsy.Crawler
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            //change to get website Id
            var website = await GetWebsite(args);
            var sesionId= await Repository.CreateNewSession();

            var feeder = new Feeder();
            var downloader = new Downloader(website.Seed);
            var processor = new Processor(website);
            feeder.AddSeed(website.Seed.ToString());

            var crawler = new Domain.Crawler(feeder, processor, downloader, website, sesionId);
            await crawler.StartAsync();
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

        private static Uri GetSeed(string[] args)
        {
            if (args.Length == 1 && Uri.TryCreate(args[0], UriKind.Absolute, out Uri seed))
            {
                return seed;
            }
            else
            {
                string userSeed;
                Uri parsedSeed;
                do
                {
                    Console.Write("Please enter a seed:");
                    userSeed = Console.ReadLine();
                    Console.Clear();

                }
                while (!Uri.TryCreate(userSeed, UriKind.Absolute, out parsedSeed));
                Console.WriteLine($"Starting crawl for {userSeed}");
                return parsedSeed;
            }
        }
    }
}
