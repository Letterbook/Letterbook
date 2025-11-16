using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddPeerRemarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrivateComment",
                table: "Peers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicRemark",
                table: "Peers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrivateComment",
                table: "Peers");

            migrationBuilder.DropColumn(
                name: "PublicRemark",
                table: "Peers");
        }
    }
}
