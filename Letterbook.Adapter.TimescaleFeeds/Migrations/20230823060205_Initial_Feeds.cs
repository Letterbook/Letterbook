using System;
using Letterbook.Adapter.TimescaleFeeds.Extensions;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.TimescaleFeeds.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Feeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feeds",
                columns: table => new
                {
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    AudienceKey = table.Column<string>(type: "text", nullable: false),
                    AudienceName = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string[]>(type: "text[]", nullable: false),
                    Authority = table.Column<string>(type: "text", nullable: false),
                    BoostedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_AudienceKey",
                table: "Feeds",
                column: "AudienceKey");

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_CreatedBy",
                table: "Feeds",
                column: "CreatedBy")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_EntityId",
                table: "Feeds",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_Time",
                table: "Feeds",
                column: "Time");

            migrationBuilder.CreateHyperTable("Feeds", "Time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feeds");
        }
    }
}
