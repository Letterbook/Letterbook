using System;
using System.Collections.Generic;
using Letterbook.Adapter.TimescaleFeeds.EntityModels;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.TimescaleFeeds.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Timelines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Preview = table.Column<string>(type: "text", nullable: false),
                    Authority = table.Column<string>(type: "text", nullable: false),
                    Creators = table.Column<List<TimelineProfile>>(type: "jsonb", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    InReplyToId = table.Column<Guid>(type: "uuid", nullable: true),
                    SharedBy = table.Column<TimelineProfile>(type: "jsonb", nullable: true),
                    AudienceId = table.Column<string>(type: "text", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timelines", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Timelines_AudienceId",
                table: "Timelines",
                column: "AudienceId")
                .Annotation("Npgsql:IndexMethod", "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_Timelines_Authority",
                table: "Timelines",
                column: "Authority")
                .Annotation("Npgsql:IndexMethod", "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_Timelines_Creators",
                table: "Timelines",
                column: "Creators")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_Timelines_PostId",
                table: "Timelines",
                column: "PostId")
                .Annotation("Npgsql:IndexMethod", "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_Timelines_Time",
                table: "Timelines",
                column: "Time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Timelines");
        }
    }
}
