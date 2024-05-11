using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.TimescaleFeeds.Migrations
{
    /// <inheritdoc />
    public partial class AudienceUriKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AudienceId",
                table: "Timelines",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "AudienceId",
                table: "Timelines",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
