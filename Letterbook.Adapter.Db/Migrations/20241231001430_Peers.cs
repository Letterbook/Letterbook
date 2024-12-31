using System;
using System.Collections.Generic;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class Peers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<IDictionary<Restrictions, DateTimeOffset>>(
                name: "Restrictions",
                table: "Profiles",
                type: "jsonb",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "Peers",
                columns: table => new
                {
                    Authority = table.Column<string>(type: "text", nullable: false),
                    Hostname = table.Column<string>(type: "text", nullable: false),
                    Restrictions = table.Column<IDictionary<Restrictions, DateTimeOffset>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peers", x => x.Authority);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Peers");

            migrationBuilder.DropColumn(
                name: "Restrictions",
                table: "Profiles");
        }
    }
}
