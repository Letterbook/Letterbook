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

            migrationBuilder.Sql(
				"""
				CREATE OR REPLACE FUNCTION array_reverse(anyarray) RETURNS anyarray AS $$
				SELECT ARRAY(
					SELECT $1[i]
					FROM generate_subscripts($1,1) AS s(i)
					ORDER BY i DESC
				);
				$$ LANGUAGE 'sql' STRICT IMMUTABLE;
				""");

            /**
             * regex:
             * \w+:\/\/				the scheme (ex. https://)
             * (.*@)?				discarded capture group for the user info, just in case
             * (\w+[\.\w+]*)		capture group for the actual host name, which is what we want
             * \/?.*				the port, path, query, and fragment portions
             */
            // \w+:\/\/(.*@)?(\w+[\.\w+]*)\/?.*
            migrationBuilder.Sql(
				"""
				UPDATE "Profiles" p
				SET "Authority"=(
					SELECT array_to_string(array_reverse(string_to_array(regexp_replace(p2."FediId", '\w+:\/\/(.*@)?(\w+[\.\w+]*)\/?.*', '\2'), '.')), '.')
					FROM "Profiles" p2
					WHERE p2."Id"=p."Id"
				)
				WHERE p."Authority" = '';
				"""
            );
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
