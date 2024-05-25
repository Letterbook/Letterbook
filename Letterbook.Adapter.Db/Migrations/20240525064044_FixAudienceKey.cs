using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class FixAudienceKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudienceProfileMembers_Audience_AudiencesId",
                table: "AudienceProfileMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_PostsToAudience_Audience_AudienceId",
                table: "PostsToAudience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostsToAudience",
                table: "PostsToAudience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AudienceProfileMembers",
                table: "AudienceProfileMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audience",
                table: "Audience");

            migrationBuilder.DropIndex(
                name: "IX_Audience_FediId",
                table: "Audience");

            migrationBuilder.DropColumn(
                name: "AudienceId",
                table: "PostsToAudience");

            migrationBuilder.DropColumn(
                name: "AudiencesId",
                table: "AudienceProfileMembers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Audience");

            migrationBuilder.AddColumn<string>(
                name: "AudienceFediId",
                table: "PostsToAudience",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AudiencesFediId",
                table: "AudienceProfileMembers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostsToAudience",
                table: "PostsToAudience",
                columns: new[] { "AudienceFediId", "PostId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AudienceProfileMembers",
                table: "AudienceProfileMembers",
                columns: new[] { "AudiencesFediId", "MembersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audience",
                table: "Audience",
                column: "FediId");

            migrationBuilder.AddForeignKey(
                name: "FK_AudienceProfileMembers_Audience_AudiencesFediId",
                table: "AudienceProfileMembers",
                column: "AudiencesFediId",
                principalTable: "Audience",
                principalColumn: "FediId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostsToAudience_Audience_AudienceFediId",
                table: "PostsToAudience",
                column: "AudienceFediId",
                principalTable: "Audience",
                principalColumn: "FediId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudienceProfileMembers_Audience_AudiencesFediId",
                table: "AudienceProfileMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_PostsToAudience_Audience_AudienceFediId",
                table: "PostsToAudience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostsToAudience",
                table: "PostsToAudience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AudienceProfileMembers",
                table: "AudienceProfileMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audience",
                table: "Audience");

            migrationBuilder.DropColumn(
                name: "AudienceFediId",
                table: "PostsToAudience");

            migrationBuilder.DropColumn(
                name: "AudiencesFediId",
                table: "AudienceProfileMembers");

            migrationBuilder.AddColumn<Guid>(
                name: "AudienceId",
                table: "PostsToAudience",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AudiencesId",
                table: "AudienceProfileMembers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Audience",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostsToAudience",
                table: "PostsToAudience",
                columns: new[] { "AudienceId", "PostId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AudienceProfileMembers",
                table: "AudienceProfileMembers",
                columns: new[] { "AudiencesId", "MembersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audience",
                table: "Audience",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Audience_FediId",
                table: "Audience",
                column: "FediId");

            migrationBuilder.AddForeignKey(
                name: "FK_AudienceProfileMembers_Audience_AudiencesId",
                table: "AudienceProfileMembers",
                column: "AudiencesId",
                principalTable: "Audience",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostsToAudience_Audience_AudienceId",
                table: "PostsToAudience",
                column: "AudienceId",
                principalTable: "Audience",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
