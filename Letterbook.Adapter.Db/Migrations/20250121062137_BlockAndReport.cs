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
            migrationBuilder.AddColumn<IDictionary<Restrictions, DateTimeOffset>>(
                name: "Restrictions",
                table: "Profiles",
                type: "jsonb",
                nullable: false,
                defaultValue: new Dictionary<Restrictions, DateTimeOffset>()
                {
	                [Restrictions.None] = new DateTimeOffset(new DateTime(2025, 2, 4, 16, 59, 41, 837, DateTimeKind.Unspecified).AddTicks(4925), new TimeSpan(0, 0, 0, 0, 0)),
                });

            migrationBuilder.AddColumn<List<RelationCondition>>(
                name: "Conditions",
                table: "FollowerRelation",
                type: "jsonb",
                nullable: false,
                defaultValue: new List<RelationCondition>());

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModerationRemark");

            migrationBuilder.DropTable(
                name: "Peers");

            migrationBuilder.DropTable(
                name: "ReportByPolicy");

            migrationBuilder.DropTable(
                name: "ReportedPost");

            migrationBuilder.DropTable(
                name: "ReportModerator");

            migrationBuilder.DropTable(
                name: "ReportSubject");

            migrationBuilder.DropTable(
                name: "ModerationPolicy");

            migrationBuilder.DropTable(
                name: "ModerationReport");

            migrationBuilder.DropColumn(
                name: "Restrictions",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Conditions",
                table: "FollowerRelation");
        }
    }
}
