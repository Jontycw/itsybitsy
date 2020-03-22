using ItsyBitsy.Domain;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ItsyBitsy.Crawler
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var seedUrl = GetSeed(args);

            var feeder = new Feeder();
            var downloader = new Downloader(seedUrl);
            var processor = new Processor(seedUrl);
            feeder.AddLink(seedUrl.ToString());

            var crawler = new Domain.Crawler(feeder, processor, downloader);
            await crawler.StartAsync();
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
                    Console.Write("Please enger a seed:");
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
