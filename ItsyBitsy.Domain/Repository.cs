using ItsyBitsy.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ItsyBitsy.Domain
{
    public class Repository
    {
        public static async Task<int> SaveLink(DownloadResult response, int websiteId, int sessionId, int? parentId)
        {
            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            var newPage = new Page()
            {
                SessionId = sessionId,
                StatusCode = response.Status,
                WebsiteId = websiteId,
                Uri = response.Uri,
                ContentType = (byte)response.ContentType,
                DownloadTime = response.DownloadTime,
                ContentLength = response.ContentLengthBytes
            };
            context.Page.Add(newPage);

            if (parentId.HasValue)
            {
                var newPageRelation = new PageRelation()
                {
                    ParentPageId = parentId.Value,
                    ChildPage = newPage,
                };
                context.PageRelation.Add(newPageRelation);
            }

            await context.SaveChangesAsync();
            return newPage.Id;
        }

        public static List<Website> GetWebsites()
        {
            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            return context.Website.Select(x => new Website(x)).ToList();
        }

        public static async Task EndSession(int sessionId)
        {
            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            var session = new Session()
            {
                Id = sessionId,
                EndTime = DateTime.Now
            };
            context.Entry(session).Property(x => x.EndTime).IsModified = true;
            await context.SaveChangesAsync();
        }

        public static async Task<int> CreateNewSession()
        {
            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            var session = new Session()
            {
                StartTime = DateTime.Now
            };
            var dbWebsite = context.Session.Add(session);
            await context.SaveChangesAsync();
            return session.Id;
        }

        public static async Task<bool> PageExists(string newLinkFound)
        {
            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            return await context.Page.AnyAsync(x => x.Uri == newLinkFound);
        }

        public static async Task<Website> CreateWebsite(string seed)
        {
            if (!Uri.TryCreate(seed, UriKind.Absolute, out Uri uri))
                throw new InvalidCastException($"{seed} is not a valid uri.");

            if (uri.Scheme != "http" && uri.Scheme != "https")
                throw new Exception("Only http and https are accepted.");

            var host = new Uri($"{uri.Scheme}://{uri.Host}");
            using var downloader = new Downloader(host);
            var downloadResult = await downloader.DownloadAsync(string.Empty);
            var websiteHomeUri = downloadResult.Redirectedto;

            if(seed != websiteHomeUri)
                throw new Exception($"{seed} redirects to {websiteHomeUri} please use this as the seed.");

            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            if (context.Website.Any(x => x.Seed == websiteHomeUri))
                throw new InvalidCastException($"{websiteHomeUri} already exists.");

            var dbWebsite = new Data.Website() { Seed = seed };
            context.Website.Add(dbWebsite);
            await context.SaveChangesAsync();

            return new Website(dbWebsite);
        }

        public static async Task<Website> GetDomainWebsite(int websiteId)
        {
            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            var dbWebsite = await context.Website.FindAsync(websiteId);
            if (dbWebsite != null)
                return new Website(dbWebsite);

            return null;
        }

        internal static async Task AddToProcessQueue(string link, int websiteId, int sessionId)
        {
            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            var newItem = new ProcessQueue()
            {
                Link = link,
                SessionId = sessionId,
                WebsiteId = websiteId,
                TimeStamp = DateTime.Now
            };
            context.ProcessQueue.Add(newItem);
            await context.SaveChangesAsync();
        }

        internal static IEnumerable<ProcessQueue> GetProcessQueueItems(int sessionId, int websiteId)
        {
            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            var nextQueueItems = (from queueItem in context.ProcessQueue
                                  where queueItem.SessionId == sessionId && queueItem.WebsiteId == websiteId
                                  orderby queueItem.TimeStamp
                                  select queueItem)
                                 .Take(400);

            return nextQueueItems.ToList();
        }

        internal static async Task RemoveQueuedItems(IEnumerable<ProcessQueue> successfullyQueued)
        {
            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            context.ProcessQueue.RemoveRange(successfullyQueued);
            await context.SaveChangesAsync();
        }

        internal static async Task AddPageRelation(IEnumerable<string> existingLinks, int parentId)
        {
            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            var pageRelations = from page in context.Page
                                where existingLinks.Contains(page.Uri)
                                select new PageRelation() { ChildPageId = page.Id, ParentPageId = parentId };

            context.PageRelation.AddRange(pageRelations);
            await context.SaveChangesAsync();
        }
    }
}
