using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ItsyBitsy.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Website",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Seed = table.Column<string>(maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Website", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uri = table.Column<string>(maxLength: 4000, nullable: false),
                    StatusCode = table.Column<string>(maxLength: 3, nullable: false),
                    ContentType = table.Column<byte>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    WebsiteId = table.Column<int>(nullable: false),
                    SessionId = table.Column<int>(nullable: false),
                    DownloadTime = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Page_Session_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Session",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Page_Website_WebsiteId",
                        column: x => x.WebsiteId,
                        principalTable: "Website",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessQueue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Link = table.Column<string>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    SessionId = table.Column<int>(nullable: false),
                    WebsiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessQueue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessQueue_Session_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Session",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessQueue_Website_WebsiteId",
                        column: x => x.WebsiteId,
                        principalTable: "Website",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PageRelation",
                columns: table => new
                {
                    ParentPageId = table.Column<int>(nullable: false),
                    ChildPageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageRelation", x => new { x.ParentPageId, x.ChildPageId });
                    table.ForeignKey(
                        name: "FK_PageRelation_Page_ChildPageId",
                        column: x => x.ChildPageId,
                        principalTable: "Page",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageRelation_Page_ParentPageId",
                        column: x => x.ParentPageId,
                        principalTable: "Page",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Page_SessionId",
                table: "Page",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Page_WebsiteId",
                table: "Page",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PageRelation_ChildPageId",
                table: "PageRelation",
                column: "ChildPageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessQueue_SessionId",
                table: "ProcessQueue",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessQueue_WebsiteId",
                table: "ProcessQueue",
                column: "WebsiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageRelation");

            migrationBuilder.DropTable(
                name: "ProcessQueue");

            migrationBuilder.DropTable(
                name: "Page");

            migrationBuilder.DropTable(
                name: "Session");

            migrationBuilder.DropTable(
                name: "Website");
        }
    }
}
