using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class FixMentions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mention_Posts_PostId",
                table: "Mention");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mention",
                table: "Mention");

            migrationBuilder.DropIndex(
                name: "IX_Mention_SubjectId",
                table: "Mention");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Mention");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Mention",
                newName: "SourceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mention",
                table: "Mention",
                columns: new[] { "SubjectId", "SourceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Mention_SourceId",
                table: "Mention",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mention_Posts_SourceId",
                table: "Mention",
                column: "SourceId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mention_Posts_SourceId",
                table: "Mention");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mention",
                table: "Mention");

            migrationBuilder.DropIndex(
                name: "IX_Mention_SourceId",
                table: "Mention");

            migrationBuilder.RenameColumn(
                name: "SourceId",
                table: "Mention",
                newName: "Id");

            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "Mention",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mention",
                table: "Mention",
                columns: new[] { "PostId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Mention_SubjectId",
                table: "Mention",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mention_Posts_PostId",
                table: "Mention",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
