using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddAudienceLabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.AddColumn<string>(
		        name: "Label",
		        table: "Audience",
		        type: "text",
		        defaultValue: "",
		        nullable: false);

	        /// Uses the last component of the audience URI as the label
	        migrationBuilder.Sql(
		        """
		        UPDATE "Audience" a
		        SET "Label"=(
		            SELECT initcap(reverse(regexp_replace(reverse("FediId"), '(\w+)[\/?#=].*', '\1')))
		            FROM "Audience" a2
		            WHERE a2."FediId" = a."FediId"
		        )
		        WHERE a."Label" = ''
		        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Label",
                table: "Audience");
        }
    }
}
