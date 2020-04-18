using ItsyBitsy.Data;
using System;
using System.Collections.Generic;
using System.Text;
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
                ParentPageId = parentId,
                SessionId = sessionId,
                StatusCode = response.Status,
                WebsiteId = websiteId,
                Uri = response.Uri,
                ContentType = response.ContentType,
                DownloadTime = response.DownloadTime
            };
            context.Page.Add(newPage);
            await context.SaveChangesAsync();
            return newPage.Id;
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
    }
}
