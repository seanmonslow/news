using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JwtAuthSampleAPI.Migrations
{
    public partial class InitialDataModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    author = table.Column<string>(nullable: true),
                    title = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    urlToImage = table.Column<string>(nullable: true),
                    publishedAt = table.Column<DateTime>(nullable: false),
                    content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleUser",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: true),
                    Articleid = table.Column<int>(nullable: true),
                    Like = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleUser", x => x.id);
                    table.ForeignKey(
                        name: "FK_ArticleUser_Article_Articleid",
                        column: x => x.Articleid,
                        principalTable: "Article",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArticleUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleUser_Articleid",
                table: "ArticleUser",
                column: "Articleid");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleUser_UserId",
                table: "ArticleUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleUser");

            migrationBuilder.DropTable(
                name: "Article");
        }
    }
}
