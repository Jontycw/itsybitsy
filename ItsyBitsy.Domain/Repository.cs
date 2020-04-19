using ItsyBitsy.Data;
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
                ContentType = response.ContentType,
                DownloadTime = response.DownloadTime
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

        public static async Task<Website> GetDomainWebsite(int websiteId)
        {
            using ItsyBitsyDbContext context = new ItsyBitsyDbContext();
            var dbWebsite = await context.Website.FindAsync(websiteId);
            if (dbWebsite != null)
                return new Website(dbWebsite);

            return null;
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
