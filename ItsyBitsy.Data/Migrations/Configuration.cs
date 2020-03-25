﻿namespace ItsyBitsy.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ItsyBitsy.Data.ItsyBitsyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ItsyBitsyDbContext context)
        {
            context.Website.AddOrUpdate(new Website()
            {
                Id = 1,
                Seed = "https://sudpave.co.za/"
            });
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
