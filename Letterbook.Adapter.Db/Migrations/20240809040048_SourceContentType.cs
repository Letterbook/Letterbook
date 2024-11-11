using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class SourceContentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Content",
                newName: "SourceText");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Content",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Html",
                table: "Content",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SourceContentType",
                table: "Content",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "Html",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "SourceContentType",
                table: "Content");

            migrationBuilder.RenameColumn(
                name: "SourceText",
                table: "Content",
                newName: "Text");
        }
    }
}
