using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ItsyBitsy.UI
{
    public sealed class CrawlContext
    {
        private static readonly Lazy<CrawlContext> lazy = new Lazy<CrawlContext>(Initialize);
        private static CrawlContext Initialize()
        {
            List<Website> websitesFromDb;
            try
            {
                websitesFromDb = Repository.GetWebsites();
            }
            catch
            {
                websitesFromDb = new List<Website>();
            }

            return new CrawlContext(websitesFromDb);
        }

        public static CrawlContext Instance { get { return lazy.Value; } }

        private CrawlContext(List<Website> websites)
        {
            WebsiteSeeds = new ObservableCollection<Website>(websites);
        }

        public ObservableCollection<Website> WebsiteSeeds { get; set; }
        public Website SelectedWebsite { get; set; }
    }
}
