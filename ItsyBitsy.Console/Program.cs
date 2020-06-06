using System;
using System.Threading;

namespace ItsyBitsy.Crawler
{
    public class Program
    {
        static readonly CrawlProgress prog = new CrawlProgress();
        static void Main(string[] args)
        {
            var gg = new Program();
            var threadDelegate = new ThreadStart(gg.IncNums);

            Thread t1 = new Thread(threadDelegate);
            Thread t2 = new Thread(threadDelegate);
            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Console.WriteLine(prog.TotalLinks);
            Console.ReadLine();
        }

        public void IncNums()
        {
            for (int i = 0; i < 100; i++)
            {
                prog.TotalLinks++;
            }
        }
    }
}
