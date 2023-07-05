using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Host = table.Column<string>(type: "text", nullable: false),
                    LocalId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audiences",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audiences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Objects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    LocalId = table.Column<string>(type: "text", nullable: true),
                    Host = table.Column<string>(type: "text", nullable: false),
                    ActorId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Objects_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActorAudience",
                columns: table => new
                {
                    AudiencesId = table.Column<string>(type: "text", nullable: false),
                    MembersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorAudience", x => new { x.AudiencesId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_ActorAudience_Actors_MembersId",
                        column: x => x.MembersId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActorAudience_Audiences_AudiencesId",
                        column: x => x.AudiencesId,
                        principalTable: "Audiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinObjectActor",
                columns: table => new
                {
                    ActorId = table.Column<string>(type: "text", nullable: false),
                    ApObject3Id = table.Column<string>(type: "text", nullable: false),
                    ObjectId = table.Column<string>(type: "text", nullable: false),
                    Relationship = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinObjectActor", x => new { x.ActorId, x.ApObject3Id });
                    table.ForeignKey(
                        name: "FK_JoinObjectActor_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinObjectActor_Objects_ApObject3Id",
                        column: x => x.ApObject3Id,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JoinObjectAudience",
                columns: table => new
                {
                    AudienceId = table.Column<string>(type: "text", nullable: false),
                    ObjectsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinObjectAudience", x => new { x.AudienceId, x.ObjectsId });
                    table.ForeignKey(
                        name: "FK_JoinObjectAudience_Audiences_AudienceId",
                        column: x => x.AudienceId,
                        principalTable: "Audiences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinObjectAudience_Objects_ObjectsId",
                        column: x => x.ObjectsId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActorAudience_MembersId",
                table: "ActorAudience",
                column: "MembersId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinObjectActor_ApObject3Id",
                table: "JoinObjectActor",
                column: "ApObject3Id");

            migrationBuilder.CreateIndex(
                name: "IX_JoinObjectAudience_ObjectsId",
                table: "JoinObjectAudience",
                column: "ObjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_ActorId",
                table: "Objects",
                column: "ActorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActorAudience");

            migrationBuilder.DropTable(
                name: "JoinObjectActor");

            migrationBuilder.DropTable(
                name: "JoinObjectAudience");

            migrationBuilder.DropTable(
                name: "Audiences");

            migrationBuilder.DropTable(
                name: "Objects");

            migrationBuilder.DropTable(
                name: "Actors");
        }
    }
}
