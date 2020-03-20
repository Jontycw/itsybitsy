namespace ItsyBitsy.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDatabase : DbMigration
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
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Session", t => t.SessionId, cascadeDelete: true)
                .ForeignKey("dbo.Website", t => t.WebsiteId, cascadeDelete: true)
                .Index(t => t.WebsiteId)
                .Index(t => t.SessionId);
            
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
                "dbo.PageLinks",
                c => new
                    {
                        PageId = c.Int(nullable: false),
                        ParentPageId = c.Int(nullable: false),
                        SessionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PageId, t.ParentPageId, t.SessionId })
                .ForeignKey("dbo.Page", t => t.PageId, cascadeDelete: true)
                .ForeignKey("dbo.Page", t => t.ParentPageId, cascadeDelete: true)
                .ForeignKey("dbo.Session", t => t.SessionId, cascadeDelete: true)
                .Index(t => t.PageId)
                .Index(t => t.ParentPageId)
                .Index(t => t.SessionId);
            
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
            DropForeignKey("dbo.PageLinks", "SessionId", "dbo.Session");
            DropForeignKey("dbo.PageLinks", "ParentPageId", "dbo.Page");
            DropForeignKey("dbo.PageLinks", "PageId", "dbo.Page");
            DropForeignKey("dbo.Page", "WebsiteId", "dbo.Website");
            DropForeignKey("dbo.Page", "SessionId", "dbo.Session");
            DropIndex("dbo.ProcessQueue", new[] { "SessionId" });
            DropIndex("dbo.PageLinks", new[] { "SessionId" });
            DropIndex("dbo.PageLinks", new[] { "ParentPageId" });
            DropIndex("dbo.PageLinks", new[] { "PageId" });
            DropIndex("dbo.Page", new[] { "SessionId" });
            DropIndex("dbo.Page", new[] { "WebsiteId" });
            DropTable("dbo.ProcessQueue");
            DropTable("dbo.PageLinks");
            DropTable("dbo.Website");
            DropTable("dbo.Session");
            DropTable("dbo.Page");
        }
    }
}
