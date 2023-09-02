using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Profiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Handle",
                table: "Profiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FollowerRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    FollowsId = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FollowersCollectionId = table.Column<string>(type: "text", nullable: false),
                    FollowingId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowerRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowerRelation_Profiles_FollowersCollectionId",
                        column: x => x.FollowersCollectionId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FollowerRelation_Profiles_FollowingId",
                        column: x => x.FollowingId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FollowerRelation_Profiles_FollowsId",
                        column: x => x.FollowsId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FollowerRelation_Profiles_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_LocalId",
                table: "Profiles",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerRelation_Date",
                table: "FollowerRelation",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerRelation_FollowersCollectionId",
                table: "FollowerRelation",
                column: "FollowersCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerRelation_FollowingId",
                table: "FollowerRelation",
                column: "FollowingId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerRelation_FollowsId",
                table: "FollowerRelation",
                column: "FollowsId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerRelation_SubjectId",
                table: "FollowerRelation",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FollowerRelation");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_LocalId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Handle",
                table: "Profiles");
        }
    }
}
