using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Accounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LocalId = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MimeType = table.Column<string>(type: "text", nullable: false),
                    FileLocation = table.Column<string>(type: "text", nullable: false),
                    Expiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LocalId = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    Client = table.Column<string>(type: "text", nullable: true),
                    InReplyToId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_Notes_InReplyToId",
                        column: x => x.InReplyToId,
                        principalTable: "Notes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LocalId = table.Column<string>(type: "text", nullable: true),
                    OwnedById = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    ImageId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Accounts_OwnedById",
                        column: x => x.OwnedById,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Profiles_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Audience",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ImageId = table.Column<string>(type: "text", nullable: true),
                    NoteId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audience_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Audience_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LinkedProfile",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "text", nullable: false),
                    ProfileId = table.Column<string>(type: "text", nullable: false),
                    Permission = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkedProfile", x => new { x.AccountId, x.ProfileId });
                    table.ForeignKey(
                        name: "FK_LinkedProfile_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LinkedProfile_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mention",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false),
                    ImageId = table.Column<string>(type: "text", nullable: true),
                    NoteId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mention", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mention_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mention_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Mention_Profiles_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Audience_ImageId",
                table: "Audience",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Audience_NoteId",
                table: "Audience",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_AudienceProfile_MembersId",
                table: "AudienceProfile",
                column: "MembersId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedProfile_ProfileId",
                table: "LinkedProfile",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Mention_ImageId",
                table: "Mention",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Mention_NoteId",
                table: "Mention",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Mention_SubjectId",
                table: "Mention",
                column: "SubjectId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Notes_InReplyToId",
                table: "Notes",
                column: "InReplyToId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_ImageId",
                table: "Profiles",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_OwnedById",
                table: "Profiles",
                column: "OwnedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AudienceProfile");

            migrationBuilder.DropTable(
                name: "LinkedProfile");

            migrationBuilder.DropTable(
                name: "Mention");

            migrationBuilder.DropTable(
                name: "NoteProfile");

            migrationBuilder.DropTable(
                name: "NoteProfile1");

            migrationBuilder.DropTable(
                name: "NoteProfile2");

            migrationBuilder.DropTable(
                name: "Audience");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Images");
        }
    }
}
