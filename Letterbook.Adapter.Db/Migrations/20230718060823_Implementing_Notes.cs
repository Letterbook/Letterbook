using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class Implementing_Notes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Notes_NoteId",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_NoteId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "Profiles");

            migrationBuilder.CreateTable(
                name: "NoteProfile",
                columns: table => new
                {
                    CreatedNotesId = table.Column<string>(type: "text", nullable: false),
                    CreatorsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteProfile", x => new { x.CreatedNotesId, x.CreatorsId });
                    table.ForeignKey(
                        name: "FK_NoteProfile_Notes_CreatedNotesId",
                        column: x => x.CreatedNotesId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoteProfile_Profiles_CreatorsId",
                        column: x => x.CreatorsId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoteProfile1",
                columns: table => new
                {
                    LikedById = table.Column<string>(type: "text", nullable: false),
                    LikedNotesId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteProfile1", x => new { x.LikedById, x.LikedNotesId });
                    table.ForeignKey(
                        name: "FK_NoteProfile1_Notes_LikedNotesId",
                        column: x => x.LikedNotesId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoteProfile1_Profiles_LikedById",
                        column: x => x.LikedById,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoteProfile2",
                columns: table => new
                {
                    BoostedById = table.Column<string>(type: "text", nullable: false),
                    BoostedNotesId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteProfile2", x => new { x.BoostedById, x.BoostedNotesId });
                    table.ForeignKey(
                        name: "FK_NoteProfile2_Notes_BoostedNotesId",
                        column: x => x.BoostedNotesId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoteProfile2_Profiles_BoostedById",
                        column: x => x.BoostedById,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NoteProfile_CreatorsId",
                table: "NoteProfile",
                column: "CreatorsId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteProfile1_LikedNotesId",
                table: "NoteProfile1",
                column: "LikedNotesId");

            migrationBuilder.CreateIndex(
                name: "IX_NoteProfile2_BoostedNotesId",
                table: "NoteProfile2",
                column: "BoostedNotesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NoteProfile");

            migrationBuilder.DropTable(
                name: "NoteProfile1");

            migrationBuilder.DropTable(
                name: "NoteProfile2");

            migrationBuilder.AddColumn<string>(
                name: "NoteId",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_NoteId",
                table: "Profiles",
                column: "NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Notes_NoteId",
                table: "Profiles",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id");
        }
    }
}
