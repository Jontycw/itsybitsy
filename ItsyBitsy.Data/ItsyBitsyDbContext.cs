using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;

namespace ItsyBitsy.Data
{
    public class ItsyBitsyDbContext : DbContext
    {
        public ItsyBitsyDbContext()
            :base("ItsyBitsyDb")
        {
        }

        protected override void OnModelCreating(DbModelBuilder dbModelBuilder)
        {
            dbModelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<Session> Session { get; set; }
        public DbSet<Website> Website { get; set; }
        public DbSet<Page> Page { get; set; }
        public DbSet<ProcessQueue> ProcessQueue { get; set; }
    }
}
