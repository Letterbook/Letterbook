using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddInviteCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvitedById",
                table: "Accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InviteCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Expiration = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RemainingUses = table.Column<int>(type: "integer", nullable: false),
                    Uses = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InviteCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InviteCodes_Accounts_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_InvitedById",
                table: "Accounts",
                column: "InvitedById");

            migrationBuilder.CreateIndex(
                name: "IX_InviteCodes_Code",
                table: "InviteCodes",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_InviteCodes_CreatedById",
                table: "InviteCodes",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_InviteCodes_InvitedById",
                table: "Accounts",
                column: "InvitedById",
                principalTable: "InviteCodes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_InviteCodes_InvitedById",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "InviteCodes");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_InvitedById",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "InvitedById",
                table: "Accounts");
        }
    }
}
