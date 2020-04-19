using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ItsyBitsy.Data
{
    public class ItsyBitsyDbContext : DbContext
    {
        public ItsyBitsyDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer("Server=.;Database=ItsyBitsyDb;Integrated Security=true");

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.Entity<PageRelation>()
                .HasKey(o => new { o.ParentPageId, o.ChildPageId });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Session> Session { get; set; }
        public DbSet<Website> Website { get; set; }
        public DbSet<Page> Page { get; set; }
        public DbSet<ProcessQueue> ProcessQueue { get; set; }
        public DbSet<PageRelation> PageRelation { get; set; }
    }
}
