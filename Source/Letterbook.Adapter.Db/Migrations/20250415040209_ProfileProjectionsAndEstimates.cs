using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class ProfileProjectionsAndEstimates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostsCreatedByProfile_Posts_CreatedPostsId",
                table: "PostsCreatedByProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostsCreatedByProfile",
                table: "PostsCreatedByProfile");

            migrationBuilder.DropIndex(
                name: "IX_PostsCreatedByProfile_CreatorsId",
                table: "PostsCreatedByProfile");

            migrationBuilder.RenameColumn(
                name: "CreatedPostsId",
                table: "PostsCreatedByProfile",
                newName: "PostsId");

            migrationBuilder.AddColumn<int>(
                name: "FollowersEstimate",
                table: "Profiles",
                type: "integer",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "FollowingEstimate",
                table: "Profiles",
                type: "integer",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "PostsEstimate",
                table: "Profiles",
                type: "integer",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostsCreatedByProfile",
                table: "PostsCreatedByProfile",
                columns: new[] { "CreatorsId", "PostsId" });

            migrationBuilder.CreateIndex(
                name: "IX_PostsCreatedByProfile_PostsId",
                table: "PostsCreatedByProfile",
                column: "PostsId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostsCreatedByProfile_Posts_PostsId",
                table: "PostsCreatedByProfile",
                column: "PostsId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostsCreatedByProfile_Posts_PostsId",
                table: "PostsCreatedByProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostsCreatedByProfile",
                table: "PostsCreatedByProfile");

            migrationBuilder.DropIndex(
                name: "IX_PostsCreatedByProfile_PostsId",
                table: "PostsCreatedByProfile");

            migrationBuilder.DropColumn(
                name: "FollowersEstimate",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "FollowingEstimate",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "PostsEstimate",
                table: "Profiles");

            migrationBuilder.RenameColumn(
                name: "PostsId",
                table: "PostsCreatedByProfile",
                newName: "CreatedPostsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostsCreatedByProfile",
                table: "PostsCreatedByProfile",
                columns: new[] { "CreatedPostsId", "CreatorsId" });

            migrationBuilder.CreateIndex(
                name: "IX_PostsCreatedByProfile_CreatorsId",
                table: "PostsCreatedByProfile",
                column: "CreatorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostsCreatedByProfile_Posts_CreatedPostsId",
                table: "PostsCreatedByProfile",
                column: "CreatedPostsId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
