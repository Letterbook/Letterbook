using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Letterbook.Adapter.Db.Migrations
{
    /// <inheritdoc />
    public partial class ReportNavigationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(
		        """
		        ALTER TABLE "ReportedPost"
		        RENAME TO "ReportSubjectPost";
		        """);

	        migrationBuilder.Sql(
		        """
		        ALTER TABLE "ReportSubject"
		        RENAME TO "ReportSubjectProfile";
		        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(
		        """
		        ALTER TABLE "ReportSubjectPost"
		        RENAME TO "ReportedPost";
		        """);

	        migrationBuilder.Sql(
		        """
		        ALTER TABLE "ReportSubjectProfile"
		        RENAME TO "ReportSubject";
		        """);
        }
    }
}
