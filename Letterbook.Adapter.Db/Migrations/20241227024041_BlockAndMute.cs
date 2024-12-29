using System.Collections.Generic;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class BlockAndMute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<RelationCondition>>(
                name: "Conditions",
                table: "FollowerRelation",
                type: "jsonb",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Conditions",
                table: "FollowerRelation");
        }
    }
}
