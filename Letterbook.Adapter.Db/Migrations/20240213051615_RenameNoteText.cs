using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class RenameNoteText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Content",
                newName: "Text");

            migrationBuilder.AlterColumn<string>(
                name: "ContentRootIdUri",
                table: "Post",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Authority",
                table: "Post",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedDate",
                table: "Post",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastSeenDate",
                table: "Post",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PublishedDate",
                table: "Post",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedDate",
                table: "Post",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Authority",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "LastSeenDate",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Content",
                newName: "Content");

            migrationBuilder.AlterColumn<string>(
                name: "ContentRootIdUri",
                table: "Post",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
