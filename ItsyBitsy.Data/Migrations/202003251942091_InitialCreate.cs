namespace ItsyBitsy.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Page",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Uri = c.String(),
                        StatusCode = c.String(maxLength: 3),
                        TimeStamp = c.DateTime(nullable: false),
                        WebsiteId = c.Int(nullable: false),
                        SessionId = c.Int(nullable: false),
                        ParentPageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Page", t => t.ParentPageId)
                .ForeignKey("dbo.Session", t => t.SessionId, cascadeDelete: true)
                .ForeignKey("dbo.Website", t => t.WebsiteId, cascadeDelete: true)
                .Index(t => t.WebsiteId)
                .Index(t => t.SessionId)
                .Index(t => t.ParentPageId);
            
            CreateTable(
                "dbo.Session",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Website",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Seed = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProcessQueue",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeStamp = c.DateTime(nullable: false),
                        SessionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Session", t => t.SessionId, cascadeDelete: true)
                .Index(t => t.SessionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProcessQueue", "SessionId", "dbo.Session");
            DropForeignKey("dbo.Page", "WebsiteId", "dbo.Website");
            DropForeignKey("dbo.Page", "SessionId", "dbo.Session");
            DropForeignKey("dbo.Page", "ParentPageId", "dbo.Page");
            DropIndex("dbo.ProcessQueue", new[] { "SessionId" });
            DropIndex("dbo.Page", new[] { "ParentPageId" });
            DropIndex("dbo.Page", new[] { "SessionId" });
            DropIndex("dbo.Page", new[] { "WebsiteId" });
            DropTable("dbo.ProcessQueue");
            DropTable("dbo.Website");
            DropTable("dbo.Session");
            DropTable("dbo.Page");
        }
    }
}
