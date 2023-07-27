using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Navigations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageProfile");

            migrationBuilder.CreateTable(
                name: "ImagesCreatedByProfile",
                columns: table => new
                {
                    CreatedImagesId = table.Column<string>(type: "text", nullable: false),
                    CreatorsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesCreatedByProfile", x => new { x.CreatedImagesId, x.CreatorsId });
                    table.ForeignKey(
                        name: "FK_ImagesCreatedByProfile_Images_CreatedImagesId",
                        column: x => x.CreatedImagesId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImagesCreatedByProfile_Profiles_CreatorsId",
                        column: x => x.CreatorsId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImagesCreatedByProfile_CreatorsId",
                table: "ImagesCreatedByProfile",
                column: "CreatorsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagesCreatedByProfile");

            migrationBuilder.CreateTable(
                name: "ImageProfile",
                columns: table => new
                {
                    CreatorsId = table.Column<string>(type: "text", nullable: false),
                    ImagesCreatedByProfileId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageProfile", x => new { x.CreatorsId, x.ImagesCreatedByProfileId });
                    table.ForeignKey(
                        name: "FK_ImageProfile_Images_ImagesCreatedByProfileId",
                        column: x => x.ImagesCreatedByProfileId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImageProfile_Profiles_CreatorsId",
                        column: x => x.CreatorsId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageProfile_ImagesCreatedByProfileId",
                table: "ImageProfile",
                column: "ImagesCreatedByProfileId");
        }
    }
}
