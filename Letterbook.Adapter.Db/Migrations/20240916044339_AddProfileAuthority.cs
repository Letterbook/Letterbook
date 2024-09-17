using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileAuthority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Authority",
                table: "Profiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Authority",
                table: "Profiles",
                column: "Authority");

            /**
             * regex:
             * \w+:\/\/				the scheme (ex. https://)
             * (.*@)?				discarded capture group for the user info, just in case
             * (\w+\.\w+[\.\w+]*)	capture group for the actual host name, which is what we want
             * \/?.*				the port, path, query, and fragment portions
             */
            // \w+:\/\/(.*@)?(\w+\.\w+[\.\w+]*)\/?.*

            // We have no servers, so no valuable data. So, we can probably skip this if necessary.
            // But, this migration would actually leave existing profiles in a bad state.
            // What we would need to do is extract, tokenize, reverse, and recombine the domain from the profile's
            // fediId as part of the migration.
            // Which is a _challenge_ to do in SQL.
            // migrationBuilder.Sql(
	            // """
	            // UPDATE "Profiles" p
	            // SET Authority=(SELECT "FediId" FROM "Profiles" p2 WHERE p2."Id"=p."Id")
	            // """
            // );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Profiles_Authority",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Authority",
                table: "Profiles");
        }
    }
}
