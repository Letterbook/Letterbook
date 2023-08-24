using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Letterbook.Adapter.TimescaleFeeds.Extensions;

public static class Migrations
{
    public static OperationBuilder<SqlOperation> CreateHyperTable(this MigrationBuilder migrationBuilder, string table,
        string column)
    {
        return migrationBuilder.Sql($"SELECT create_hypertable( '\"{table}\"', '{column}');");
    }
}