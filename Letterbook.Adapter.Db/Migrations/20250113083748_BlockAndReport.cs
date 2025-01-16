using System;
using System.Collections.Generic;
using Letterbook.Core.Models;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class BlockAndReport : Migration
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
                name: "ModerationPolicy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Retired = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    Policy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationPolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Peers",
                columns: table => new
                {
                    Authority = table.Column<string>(type: "text", nullable: false),
                    Hostname = table.Column<string>(type: "text", nullable: false),
                    Restrictions = table.Column<IDictionary<Restrictions, DateTimeOffset>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peers", x => x.Authority);
                });

            migrationBuilder.CreateTable(
                name: "Threads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RootId = table.Column<Guid>(type: "uuid", nullable: false),
                    FediId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Threads", x => x.Id);
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FediId = table.Column<string>(type: "text", nullable: false),
                    Inbox = table.Column<string>(type: "text", nullable: false),
                    Outbox = table.Column<string>(type: "text", nullable: false),
                    SharedInbox = table.Column<string>(type: "text", nullable: true),
                    Followers = table.Column<string>(type: "text", nullable: false),
                    Following = table.Column<string>(type: "text", nullable: false),
                    Authority = table.Column<string>(type: "text", nullable: false),
                    Handle = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CustomFields = table.Column<CustomField[]>(type: "jsonb", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OwnedById = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Restrictions = table.Column<IDictionary<Restrictions, DateTimeOffset>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
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
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentRootIdUri = table.Column<string>(type: "text", nullable: true),
                    FediId = table.Column<string>(type: "text", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    Preview = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Hostname = table.Column<string>(type: "text", nullable: false),
                    Authority = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PublishedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastSeenDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Client = table.Column<string>(type: "text", nullable: true),
                    InReplyToId = table.Column<Guid>(type: "uuid", nullable: true),
                    Replies = table.Column<string>(type: "text", nullable: true),
                    Likes = table.Column<string>(type: "text", nullable: true),
                    Shares = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Posts_InReplyToId",
                        column: x => x.InReplyToId,
                        principalTable: "Posts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Posts_Threads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "Threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Audience",
                columns: table => new
                {
                    FediId = table.Column<string>(type: "text", nullable: false),
                    SourceId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audience", x => x.FediId);
                    table.ForeignKey(
                        name: "FK_Audience_Profiles_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FollowerRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowerId = table.Column<Guid>(type: "uuid", nullable: false),
                    FollowsId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Conditions = table.Column<List<RelationCondition>>(type: "jsonb", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowerRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowerRelation_Profiles_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FollowerRelation_Profiles_FollowsId",
                        column: x => x.FollowsId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModerationReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    ContextId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReporterId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Closed = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModerationReport_Profiles_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ModerationReport_Threads_ContextId",
                        column: x => x.ContextId,
                        principalTable: "Threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    Claims = table.Column<List<ProfileClaim>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileClaims_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileClaims_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
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
                    ProfileId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SigningKey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SigningKey_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
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
                    SortKey = table.Column<int>(type: "integer", nullable: true),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Html = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    SourceText = table.Column<string>(type: "text", nullable: true),
                    SourceContentType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Content", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Content_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mention",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mention", x => new { x.PostId, x.Id });
                    table.ForeignKey(
                        name: "FK_Mention_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mention_Profiles_SubjectId",
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
                    CreatorsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsCreatedByProfile", x => new { x.CreatedPostsId, x.CreatorsId });
                    table.ForeignKey(
                        name: "FK_PostsCreatedByProfile_Posts_CreatedPostsId",
                        column: x => x.CreatedPostsId,
                        principalTable: "Posts",
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
                    LikesCollectionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsLikedByProfile", x => new { x.LikedPostsId, x.LikesCollectionId });
                    table.ForeignKey(
                        name: "FK_PostsLikedByProfile_Posts_LikedPostsId",
                        column: x => x.LikedPostsId,
                        principalTable: "Posts",
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
                    SharesCollectionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsSharedByProfile", x => new { x.SharedPostsId, x.SharesCollectionId });
                    table.ForeignKey(
                        name: "FK_PostsSharedByProfile_Posts_SharedPostsId",
                        column: x => x.SharedPostsId,
                        principalTable: "Posts",
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
                name: "AudienceProfileMembers",
                columns: table => new
                {
                    AudiencesFediId = table.Column<string>(type: "text", nullable: false),
                    MembersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudienceProfileMembers", x => new { x.AudiencesFediId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_AudienceProfileMembers_Audience_AudiencesFediId",
                        column: x => x.AudiencesFediId,
                        principalTable: "Audience",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AudienceProfileMembers_Profiles_MembersId",
                        column: x => x.MembersId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostsToAudience",
                columns: table => new
                {
                    AudienceFediId = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsToAudience", x => new { x.AudienceFediId, x.PostId });
                    table.ForeignKey(
                        name: "FK_PostsToAudience_Audience_AudienceFediId",
                        column: x => x.AudienceFediId,
                        principalTable: "Audience",
                        principalColumn: "FediId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsToAudience_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModerationRemark",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationRemark", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModerationRemark_Accounts_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModerationRemark_ModerationReport_ReportId",
                        column: x => x.ReportId,
                        principalTable: "ModerationReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportByPolicy",
                columns: table => new
                {
                    PoliciesId = table.Column<int>(type: "integer", nullable: false),
                    RelatedReportsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportByPolicy", x => new { x.PoliciesId, x.RelatedReportsId });
                    table.ForeignKey(
                        name: "FK_ReportByPolicy_ModerationPolicy_PoliciesId",
                        column: x => x.PoliciesId,
                        principalTable: "ModerationPolicy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportByPolicy_ModerationReport_RelatedReportsId",
                        column: x => x.RelatedReportsId,
                        principalTable: "ModerationReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportedPost",
                columns: table => new
                {
                    RelatedPostsId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelatedReportsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportedPost", x => new { x.RelatedPostsId, x.RelatedReportsId });
                    table.ForeignKey(
                        name: "FK_ReportedPost_ModerationReport_RelatedReportsId",
                        column: x => x.RelatedReportsId,
                        principalTable: "ModerationReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportedPost_Posts_RelatedPostsId",
                        column: x => x.RelatedPostsId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportModerator",
                columns: table => new
                {
                    ModeratedReportsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModeratorsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportModerator", x => new { x.ModeratedReportsId, x.ModeratorsId });
                    table.ForeignKey(
                        name: "FK_ReportModerator_Accounts_ModeratorsId",
                        column: x => x.ModeratorsId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportModerator_ModerationReport_ModeratedReportsId",
                        column: x => x.ModeratedReportsId,
                        principalTable: "ModerationReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportSubject",
                columns: table => new
                {
                    ReportSubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSubject", x => new { x.ReportSubjectId, x.SubjectsId });
                    table.ForeignKey(
                        name: "FK_ReportSubject_ModerationReport_ReportSubjectId",
                        column: x => x.ReportSubjectId,
                        principalTable: "ModerationReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportSubject_Profiles_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Audience_SourceId",
                table: "Audience",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_AudienceProfileMembers_MembersId",
                table: "AudienceProfileMembers",
                column: "MembersId");

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
                name: "IX_FollowerRelation_FollowerId",
                table: "FollowerRelation",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowerRelation_FollowsId",
                table: "FollowerRelation",
                column: "FollowsId");

            migrationBuilder.CreateIndex(
                name: "IX_Mention_SubjectId",
                table: "Mention",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationRemark_AuthorId",
                table: "ModerationRemark",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationRemark_ReportId",
                table: "ModerationRemark",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReport_ContextId",
                table: "ModerationReport",
                column: "ContextId");

            migrationBuilder.CreateIndex(
                name: "IX_ModerationReport_ReporterId",
                table: "ModerationReport",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ContentRootIdUri",
                table: "Posts",
                column: "ContentRootIdUri");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_FediId",
                table: "Posts",
                column: "FediId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_InReplyToId",
                table: "Posts",
                column: "InReplyToId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ThreadId",
                table: "Posts",
                column: "ThreadId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ProfileClaims_AccountId",
                table: "ProfileClaims",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileClaims_ProfileId",
                table: "ProfileClaims",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Authority",
                table: "Profiles",
                column: "Authority");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_FediId",
                table: "Profiles",
                column: "FediId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_OwnedById",
                table: "Profiles",
                column: "OwnedById");

            migrationBuilder.CreateIndex(
                name: "IX_ReportByPolicy_RelatedReportsId",
                table: "ReportByPolicy",
                column: "RelatedReportsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportedPost_RelatedReportsId",
                table: "ReportedPost",
                column: "RelatedReportsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportModerator_ModeratorsId",
                table: "ReportModerator",
                column: "ModeratorsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSubject_SubjectsId",
                table: "ReportSubject",
                column: "SubjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_SigningKey_ProfileId",
                table: "SigningKey",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Threads_FediId",
                table: "Threads",
                column: "FediId");

            migrationBuilder.CreateIndex(
                name: "IX_Threads_RootId",
                table: "Threads",
                column: "RootId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "Mention");

            migrationBuilder.DropTable(
                name: "ModerationRemark");

            migrationBuilder.DropTable(
                name: "Peers");

            migrationBuilder.DropTable(
                name: "PostsCreatedByProfile");

            migrationBuilder.DropTable(
                name: "PostsLikedByProfile");

            migrationBuilder.DropTable(
                name: "PostsSharedByProfile");

            migrationBuilder.DropTable(
                name: "PostsToAudience");

            migrationBuilder.DropTable(
                name: "ProfileClaims");

            migrationBuilder.DropTable(
                name: "ReportByPolicy");

            migrationBuilder.DropTable(
                name: "ReportedPost");

            migrationBuilder.DropTable(
                name: "ReportModerator");

            migrationBuilder.DropTable(
                name: "ReportSubject");

            migrationBuilder.DropTable(
                name: "SigningKey");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "AspnetIdentity");

            migrationBuilder.DropTable(
                name: "Audience");

            migrationBuilder.DropTable(
                name: "ModerationPolicy");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "ModerationReport");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Threads");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
