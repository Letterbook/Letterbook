using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class FixFollowRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowerRelation_Profiles_SubjectId",
                table: "FollowerRelation");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "FollowerRelation",
                newName: "FollowerId");

            migrationBuilder.RenameIndex(
                name: "IX_FollowerRelation_SubjectId",
                table: "FollowerRelation",
                newName: "IX_FollowerRelation_FollowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowerRelation_Profiles_FollowerId",
                table: "FollowerRelation",
                column: "FollowerId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowerRelation_Profiles_FollowerId",
                table: "FollowerRelation");

            migrationBuilder.RenameColumn(
                name: "FollowerId",
                table: "FollowerRelation",
                newName: "SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_FollowerRelation_FollowerId",
                table: "FollowerRelation",
                newName: "IX_FollowerRelation_SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowerRelation_Profiles_SubjectId",
                table: "FollowerRelation",
                column: "SubjectId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
