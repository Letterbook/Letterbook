using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class ThreadEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Post_Thread",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "Thread",
                table: "Post");

            migrationBuilder.AddColumn<Guid>(
                name: "ThreadId",
                table: "Post",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ThreadContext",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdUri = table.Column<string>(type: "text", nullable: false),
                    RootId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadContext", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThreadContext_Post_RootId",
                        column: x => x.RootId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_ThreadId",
                table: "Post",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadContext_IdUri",
                table: "ThreadContext",
                column: "IdUri");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadContext_RootId",
                table: "ThreadContext",
                column: "RootId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_ThreadContext_ThreadId",
                table: "Post",
                column: "ThreadId",
                principalTable: "ThreadContext",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_ThreadContext_ThreadId",
                table: "Post");

            migrationBuilder.DropTable(
                name: "ThreadContext");

            migrationBuilder.DropIndex(
                name: "IX_Post_ThreadId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "ThreadId",
                table: "Post");

            migrationBuilder.AddColumn<string>(
                name: "Thread",
                table: "Post",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Thread",
                table: "Post",
                column: "Thread");
        }
    }
}
