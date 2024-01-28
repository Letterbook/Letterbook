using System;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class ResetMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AspnetIdentity");

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "text", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "AspnetIdentity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "AspnetIdentity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FediId = table.Column<string>(type: "text", nullable: false),
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
                name: "AspNetUserLogins",
                schema: "AspnetIdentity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_Accounts_UserId",
                        column: x => x.UserId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "AspnetIdentity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_Accounts_UserId",
                        column: x => x.UserId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    FediId = table.Column<string>(type: "text", nullable: false),
                    Inbox = table.Column<string>(type: "text", nullable: false),
                    Outbox = table.Column<string>(type: "text", nullable: false),
                    SharedInbox = table.Column<string>(type: "text", nullable: true),
                    Followers = table.Column<string>(type: "text", nullable: false),
                    Following = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Handle = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CustomFields = table.Column<CustomField[]>(type: "jsonb", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OwnedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.FediId);
                    table.ForeignKey(
                        name: "FK_Profiles_Accounts_OwnedById",
                        column: x => x.OwnedById,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "AspnetIdentity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "AspnetIdentity",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "AspnetIdentity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_Accounts_UserId",
                        column: x => x.UserId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "AspnetIdentity",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Audience",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FediId = table.Column<string>(type: "text", nullable: false),
                    SourceFediId = table.Column<string>(type: "text", nullable: true),
                    ImageId = table.Column<Guid>(type: "uuid", nullable: true)
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
                        name: "FK_Audience_Profiles_SourceFediId",
                        column: x => x.SourceFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId");
                });

            migrationBuilder.CreateTable(
                name: "FollowerRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowerFediId = table.Column<string>(type: "text", nullable: false),
                    FollowsFediId = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowerRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowerRelation_Profiles_FollowerFediId",
                        column: x => x.FollowerFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FollowerRelation_Profiles_FollowsFediId",
                        column: x => x.FollowsFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images_Mentions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectFediId = table.Column<string>(type: "text", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images_Mentions", x => new { x.ImageId, x.Id });
                    table.ForeignKey(
                        name: "FK_Images_Mentions_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Images_Mentions_Profiles_SubjectFediId",
                        column: x => x.SubjectFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImagesCreatedByProfile",
                columns: table => new
                {
                    CreatedImagesId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorsFediId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesCreatedByProfile", x => new { x.CreatedImagesId, x.CreatorsFediId });
                    table.ForeignKey(
                        name: "FK_ImagesCreatedByProfile_Images_CreatedImagesId",
                        column: x => x.CreatedImagesId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImagesCreatedByProfile_Profiles_CreatorsFediId",
                        column: x => x.CreatorsFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinkedProfile",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileFediId = table.Column<string>(type: "text", nullable: false),
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
                        name: "FK_LinkedProfile_Profiles_ProfileFediId",
                        column: x => x.ProfileFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SigningKey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyOrder = table.Column<int>(type: "integer", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: true),
                    Family = table.Column<int>(type: "integer", nullable: false),
                    PublicKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    PrivateKey = table.Column<byte[]>(type: "bytea", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Expires = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FediId = table.Column<string>(type: "text", nullable: false),
                    ProfileFediId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SigningKey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SigningKey_Profiles_ProfileFediId",
                        column: x => x.ProfileFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId");
                });

            migrationBuilder.CreateTable(
                name: "AudienceProfileMembers",
                columns: table => new
                {
                    AudiencesId = table.Column<Guid>(type: "uuid", nullable: false),
                    MembersFediId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudienceProfileMembers", x => new { x.AudiencesId, x.MembersFediId });
                    table.ForeignKey(
                        name: "FK_AudienceProfileMembers_Audience_AudiencesId",
                        column: x => x.AudiencesId,
                        principalTable: "Audience",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudienceProfileMembers_Profiles_MembersFediId",
                        column: x => x.MembersFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Content",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FediId = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    Preview = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Discriminator = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Content", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentRootIdUri = table.Column<string>(type: "text", nullable: false),
                    FediId = table.Column<string>(type: "text", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    SubjectFediId = table.Column<string>(type: "text", nullable: false),
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
                        name: "FK_Post_AddressedTo_Profiles_SubjectFediId",
                        column: x => x.SubjectFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsCreatedByProfile",
                columns: table => new
                {
                    CreatedPostsId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorsFediId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsCreatedByProfile", x => new { x.CreatedPostsId, x.CreatorsFediId });
                    table.ForeignKey(
                        name: "FK_PostsCreatedByProfile_Post_CreatedPostsId",
                        column: x => x.CreatedPostsId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsCreatedByProfile_Profiles_CreatorsFediId",
                        column: x => x.CreatorsFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsLikedByProfile",
                columns: table => new
                {
                    LikedPostsId = table.Column<Guid>(type: "uuid", nullable: false),
                    LikesCollectionFediId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsLikedByProfile", x => new { x.LikedPostsId, x.LikesCollectionFediId });
                    table.ForeignKey(
                        name: "FK_PostsLikedByProfile_Post_LikedPostsId",
                        column: x => x.LikedPostsId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsLikedByProfile_Profiles_LikesCollectionFediId",
                        column: x => x.LikesCollectionFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsSharedByProfile",
                columns: table => new
                {
                    SharedPostsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SharesCollectionFediId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsSharedByProfile", x => new { x.SharedPostsId, x.SharesCollectionFediId });
                    table.ForeignKey(
                        name: "FK_PostsSharedByProfile_Post_SharedPostsId",
                        column: x => x.SharedPostsId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsSharedByProfile_Profiles_SharesCollectionFediId",
                        column: x => x.SharesCollectionFediId,
                        principalTable: "Profiles",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsToAudience",
                columns: table => new
                {
                    AudienceId = table.Column<Guid>(type: "uuid", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "ThreadContext",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FediId = table.Column<string>(type: "text", nullable: false),
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
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserName",
                table: "Accounts",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "AspnetIdentity",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "AspnetIdentity",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "AspnetIdentity",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "AspnetIdentity",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "AspnetIdentity",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Audience_FediId",
                table: "Audience",
                column: "FediId");

            migrationBuilder.CreateIndex(
                name: "IX_Audience_ImageId",
                table: "Audience",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Audience_SourceFediId",
                table: "Audience",
                column: "SourceFediId");

            migrationBuilder.CreateIndex(
                name: "IX_AudienceProfileMembers_MembersFediId",
                table: "AudienceProfileMembers",
                column: "MembersFediId");

            migrationBuilder.CreateIndex(
                name: "IX_Content_FediId",
                table: "Content",
                column: "FediId");

            migrationBuilder.CreateIndex(
                name: "IX_Content_PostId",
                table: "Content",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerRelation_Date",
                table: "FollowerRelation",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerRelation_FollowerFediId",
                table: "FollowerRelation",
                column: "FollowerFediId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerRelation_FollowsFediId",
                table: "FollowerRelation",
                column: "FollowsFediId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_Mentions_SubjectFediId",
                table: "Images_Mentions",
                column: "SubjectFediId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagesCreatedByProfile_CreatorsFediId",
                table: "ImagesCreatedByProfile",
                column: "CreatorsFediId");

            migrationBuilder.CreateIndex(
                name: "IX_LinkedProfile_ProfileFediId",
                table: "LinkedProfile",
                column: "ProfileFediId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ContentRootIdUri",
                table: "Post",
                column: "ContentRootIdUri");

            migrationBuilder.CreateIndex(
                name: "IX_Post_FediId",
                table: "Post",
                column: "FediId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_InReplyToId",
                table: "Post",
                column: "InReplyToId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ThreadId",
                table: "Post",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_AddressedTo_SubjectFediId",
                table: "Post_AddressedTo",
                column: "SubjectFediId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsCreatedByProfile_CreatorsFediId",
                table: "PostsCreatedByProfile",
                column: "CreatorsFediId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsLikedByProfile_LikesCollectionFediId",
                table: "PostsLikedByProfile",
                column: "LikesCollectionFediId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsSharedByProfile_SharesCollectionFediId",
                table: "PostsSharedByProfile",
                column: "SharesCollectionFediId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsToAudience_PostId",
                table: "PostsToAudience",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Id",
                table: "Profiles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_OwnedById",
                table: "Profiles",
                column: "OwnedById");

            migrationBuilder.CreateIndex(
                name: "IX_SigningKey_ProfileFediId",
                table: "SigningKey",
                column: "ProfileFediId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadContext_FediId",
                table: "ThreadContext",
                column: "FediId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadContext_RootId",
                table: "ThreadContext",
                column: "RootId");

            migrationBuilder.AddForeignKey(
                name: "FK_Content_Post_PostId",
                table: "Content",
                column: "PostId",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_ThreadContext_Post_RootId",
                table: "ThreadContext");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "AspnetIdentity");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "AspnetIdentity");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "AspnetIdentity");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "AspnetIdentity");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "AspnetIdentity");

            migrationBuilder.DropTable(
                name: "AudienceProfileMembers");

            migrationBuilder.DropTable(
                name: "Content");

            migrationBuilder.DropTable(
                name: "FollowerRelation");

            migrationBuilder.DropTable(
                name: "Images_Mentions");

            migrationBuilder.DropTable(
                name: "ImagesCreatedByProfile");

            migrationBuilder.DropTable(
                name: "LinkedProfile");

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
                name: "SigningKey");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "AspnetIdentity");

            migrationBuilder.DropTable(
                name: "Audience");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "ThreadContext");
        }
    }
}
