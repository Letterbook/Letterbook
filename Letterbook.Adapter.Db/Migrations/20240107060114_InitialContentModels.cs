using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialContentModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audience_Notes_NoteId",
                table: "Audience");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Notes_InReplyToId",
                table: "Notes");

            migrationBuilder.DropTable(
                name: "Notes_Mentions");

            migrationBuilder.DropTable(
                name: "NotesBoostedByProfile");

            migrationBuilder.DropTable(
                name: "NotesCreatedByProfile");

            migrationBuilder.DropTable(
                name: "NotesLikedByProfile");

            migrationBuilder.DropIndex(
                name: "IX_Audience_NoteId",
                table: "Audience");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notes",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_InReplyToId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "Audience");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "LocalId",
                table: "Notes");

            migrationBuilder.RenameTable(
                name: "Notes",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "InReplyToId",
                table: "Content",
                newName: "Source");

            migrationBuilder.RenameColumn(
                name: "Client",
                table: "Content",
                newName: "Preview");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Content",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Content",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Content",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdUri",
                table: "Content",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "Content",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Content",
                table: "Content",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentRootId = table.Column<Guid>(type: "uuid", nullable: false),
                    IdUri = table.Column<string>(type: "text", nullable: false),
                    Thread = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    Preview = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Client = table.Column<string>(type: "text", nullable: true),
                    InReplyToId = table.Column<Guid>(type: "uuid", nullable: true),
                    Replies = table.Column<string>(type: "text", nullable: true),
                    Likes = table.Column<string>(type: "text", nullable: true),
                    Shares = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Post_InReplyToId",
                        column: x => x.InReplyToId,
                        principalTable: "Post",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Post_AddressedTo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post_AddressedTo", x => new { x.PostId, x.Id });
                    table.ForeignKey(
                        name: "FK_Post_AddressedTo_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_AddressedTo_Profiles_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsCreatedByProfile",
                columns: table => new
                {
                    CreatedPostsId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsCreatedByProfile", x => new { x.CreatedPostsId, x.CreatorsId });
                    table.ForeignKey(
                        name: "FK_PostsCreatedByProfile_Post_CreatedPostsId",
                        column: x => x.CreatedPostsId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsCreatedByProfile_Profiles_CreatorsId",
                        column: x => x.CreatorsId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsLikedByProfile",
                columns: table => new
                {
                    LikedPostsId = table.Column<Guid>(type: "uuid", nullable: false),
                    LikesCollectionId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsLikedByProfile", x => new { x.LikedPostsId, x.LikesCollectionId });
                    table.ForeignKey(
                        name: "FK_PostsLikedByProfile_Post_LikedPostsId",
                        column: x => x.LikedPostsId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsLikedByProfile_Profiles_LikesCollectionId",
                        column: x => x.LikesCollectionId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsSharedByProfile",
                columns: table => new
                {
                    SharedPostsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SharesCollectionId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsSharedByProfile", x => new { x.SharedPostsId, x.SharesCollectionId });
                    table.ForeignKey(
                        name: "FK_PostsSharedByProfile_Post_SharedPostsId",
                        column: x => x.SharedPostsId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsSharedByProfile_Profiles_SharesCollectionId",
                        column: x => x.SharesCollectionId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsToAudience",
                columns: table => new
                {
                    AudienceId = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsToAudience", x => new { x.AudienceId, x.PostId });
                    table.ForeignKey(
                        name: "FK_PostsToAudience_Audience_AudienceId",
                        column: x => x.AudienceId,
                        principalTable: "Audience",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsToAudience_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Content_IdUri",
                table: "Content",
                column: "IdUri");

            migrationBuilder.CreateIndex(
                name: "IX_Content_PostId",
                table: "Content",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ContentRootId",
                table: "Post",
                column: "ContentRootId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_IdUri",
                table: "Post",
                column: "IdUri");

            migrationBuilder.CreateIndex(
                name: "IX_Post_InReplyToId",
                table: "Post",
                column: "InReplyToId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_Thread",
                table: "Post",
                column: "Thread");

            migrationBuilder.CreateIndex(
                name: "IX_Post_AddressedTo_SubjectId",
                table: "Post_AddressedTo",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsCreatedByProfile_CreatorsId",
                table: "PostsCreatedByProfile",
                column: "CreatorsId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsLikedByProfile_LikesCollectionId",
                table: "PostsLikedByProfile",
                column: "LikesCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsSharedByProfile_SharesCollectionId",
                table: "PostsSharedByProfile",
                column: "SharesCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsToAudience_PostId",
                table: "PostsToAudience",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Content_Post_PostId",
                table: "Content",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Content_Post_PostId",
                table: "Content");

            migrationBuilder.DropTable(
                name: "Post_AddressedTo");

            migrationBuilder.DropTable(
                name: "PostsCreatedByProfile");

            migrationBuilder.DropTable(
                name: "PostsLikedByProfile");

            migrationBuilder.DropTable(
                name: "PostsSharedByProfile");

            migrationBuilder.DropTable(
                name: "PostsToAudience");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Content",
                table: "Content");

            migrationBuilder.DropIndex(
                name: "IX_Content_IdUri",
                table: "Content");

            migrationBuilder.DropIndex(
                name: "IX_Content_PostId",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "IdUri",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Content");

            migrationBuilder.RenameTable(
                name: "Content",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "Source",
                table: "Notes",
                newName: "InReplyToId");

            migrationBuilder.RenameColumn(
                name: "Preview",
                table: "Notes",
                newName: "Client");

            migrationBuilder.AddColumn<string>(
                name: "NoteId",
                table: "Audience",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Notes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Notes",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Notes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "LocalId",
                table: "Notes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notes",
                table: "Notes",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Notes_Mentions",
                columns: table => new
                {
                    NoteId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes_Mentions", x => new { x.NoteId, x.Id });
                    table.ForeignKey(
                        name: "FK_Notes_Mentions_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_Mentions_Profiles_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotesBoostedByProfile",
                columns: table => new
                {
                    BoostedById = table.Column<string>(type: "text", nullable: false),
                    BoostedNotesId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotesBoostedByProfile", x => new { x.BoostedById, x.BoostedNotesId });
                    table.ForeignKey(
                        name: "FK_NotesBoostedByProfile_Notes_BoostedNotesId",
                        column: x => x.BoostedNotesId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotesBoostedByProfile_Profiles_BoostedById",
                        column: x => x.BoostedById,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotesCreatedByProfile",
                columns: table => new
                {
                    CreatedNotesId = table.Column<string>(type: "text", nullable: false),
                    CreatorsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotesCreatedByProfile", x => new { x.CreatedNotesId, x.CreatorsId });
                    table.ForeignKey(
                        name: "FK_NotesCreatedByProfile_Notes_CreatedNotesId",
                        column: x => x.CreatedNotesId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotesCreatedByProfile_Profiles_CreatorsId",
                        column: x => x.CreatorsId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotesLikedByProfile",
                columns: table => new
                {
                    LikedById = table.Column<string>(type: "text", nullable: false),
                    LikedNotesId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotesLikedByProfile", x => new { x.LikedById, x.LikedNotesId });
                    table.ForeignKey(
                        name: "FK_NotesLikedByProfile_Notes_LikedNotesId",
                        column: x => x.LikedNotesId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotesLikedByProfile_Profiles_LikedById",
                        column: x => x.LikedById,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audience_NoteId",
                table: "Audience",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_InReplyToId",
                table: "Notes",
                column: "InReplyToId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_Mentions_SubjectId",
                table: "Notes_Mentions",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_NotesBoostedByProfile_BoostedNotesId",
                table: "NotesBoostedByProfile",
                column: "BoostedNotesId");

            migrationBuilder.CreateIndex(
                name: "IX_NotesCreatedByProfile_CreatorsId",
                table: "NotesCreatedByProfile",
                column: "CreatorsId");

            migrationBuilder.CreateIndex(
                name: "IX_NotesLikedByProfile_LikedNotesId",
                table: "NotesLikedByProfile",
                column: "LikedNotesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Audience_Notes_NoteId",
                table: "Audience",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Notes_InReplyToId",
                table: "Notes",
                column: "InReplyToId",
                principalTable: "Notes",
                principalColumn: "Id");
        }
    }
}
