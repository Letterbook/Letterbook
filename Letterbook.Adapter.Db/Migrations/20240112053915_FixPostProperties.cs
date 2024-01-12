using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class FixPostProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audience_Profiles_SourceId",
                table: "Audience");

            migrationBuilder.DropIndex(
                name: "IX_Post_ContentRootId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "ContentRootId",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "IdUri",
                table: "ThreadContext",
                newName: "FediId");

            migrationBuilder.RenameIndex(
                name: "IX_ThreadContext_IdUri",
                table: "ThreadContext",
                newName: "IX_ThreadContext_FediId");

            migrationBuilder.AddColumn<string>(
                name: "ContentRootIdUri",
                table: "Post",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "SourceId",
                table: "Audience",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ContentRootIdUri",
                table: "Post",
                column: "ContentRootIdUri");

            migrationBuilder.AddForeignKey(
                name: "FK_Audience_Profiles_SourceId",
                table: "Audience",
                column: "SourceId",
                principalTable: "Profiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audience_Profiles_SourceId",
                table: "Audience");

            migrationBuilder.DropIndex(
                name: "IX_Post_ContentRootIdUri",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "ContentRootIdUri",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "FediId",
                table: "ThreadContext",
                newName: "IdUri");

            migrationBuilder.RenameIndex(
                name: "IX_ThreadContext_FediId",
                table: "ThreadContext",
                newName: "IX_ThreadContext_IdUri");

            migrationBuilder.AddColumn<Guid>(
                name: "ContentRootId",
                table: "Post",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "SourceId",
                table: "Audience",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_ContentRootId",
                table: "Post",
                column: "ContentRootId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audience_Profiles_SourceId",
                table: "Audience",
                column: "SourceId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
