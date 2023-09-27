using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class TrackAudienceSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudienceProfileMembers_Audience_AudienceId",
                table: "AudienceProfileMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_AudienceProfileMembers_Profiles_ProfileId",
                table: "AudienceProfileMembers");

            migrationBuilder.DropTable(
                name: "AudienceProfile");

            migrationBuilder.RenameColumn(
                name: "ProfileId",
                table: "AudienceProfileMembers",
                newName: "MembersId");

            migrationBuilder.RenameColumn(
                name: "AudienceId",
                table: "AudienceProfileMembers",
                newName: "AudiencesId");

            migrationBuilder.RenameIndex(
                name: "IX_AudienceProfileMembers_ProfileId",
                table: "AudienceProfileMembers",
                newName: "IX_AudienceProfileMembers_MembersId");

            migrationBuilder.AddColumn<string>(
                name: "SourceId",
                table: "Audience",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Audience_SourceId",
                table: "Audience",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audience_Profiles_SourceId",
                table: "Audience",
                column: "SourceId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AudienceProfileMembers_Audience_AudiencesId",
                table: "AudienceProfileMembers",
                column: "AudiencesId",
                principalTable: "Audience",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AudienceProfileMembers_Profiles_MembersId",
                table: "AudienceProfileMembers",
                column: "MembersId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audience_Profiles_SourceId",
                table: "Audience");

            migrationBuilder.DropForeignKey(
                name: "FK_AudienceProfileMembers_Audience_AudiencesId",
                table: "AudienceProfileMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_AudienceProfileMembers_Profiles_MembersId",
                table: "AudienceProfileMembers");

            migrationBuilder.DropIndex(
                name: "IX_Audience_SourceId",
                table: "Audience");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "Audience");

            migrationBuilder.RenameColumn(
                name: "MembersId",
                table: "AudienceProfileMembers",
                newName: "ProfileId");

            migrationBuilder.RenameColumn(
                name: "AudiencesId",
                table: "AudienceProfileMembers",
                newName: "AudienceId");

            migrationBuilder.RenameIndex(
                name: "IX_AudienceProfileMembers_MembersId",
                table: "AudienceProfileMembers",
                newName: "IX_AudienceProfileMembers_ProfileId");

            migrationBuilder.CreateTable(
                name: "AudienceProfile",
                columns: table => new
                {
                    AudiencesId = table.Column<string>(type: "text", nullable: false),
                    MembersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudienceProfile", x => new { x.AudiencesId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_AudienceProfile_Audience_AudiencesId",
                        column: x => x.AudiencesId,
                        principalTable: "Audience",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudienceProfile_Profiles_MembersId",
                        column: x => x.MembersId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AudienceProfile_MembersId",
                table: "AudienceProfile",
                column: "MembersId");

            migrationBuilder.AddForeignKey(
                name: "FK_AudienceProfileMembers_Audience_AudienceId",
                table: "AudienceProfileMembers",
                column: "AudienceId",
                principalTable: "Audience",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AudienceProfileMembers_Profiles_ProfileId",
                table: "AudienceProfileMembers",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
