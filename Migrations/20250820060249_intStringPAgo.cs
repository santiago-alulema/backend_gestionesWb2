using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class intStringPAgo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Paso 1: quitar identity (necesario antes de cambiar de int a uuid)
            migrationBuilder.Sql(@"ALTER TABLE ""Pagos"" DROP COLUMN ""IdPago"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Pagos"" ADD COLUMN ""IdPago"" uuid NOT NULL DEFAULT gen_random_uuid();");
            migrationBuilder.Sql(@"ALTER TABLE ""Pagos"" ADD PRIMARY KEY (""IdPago"");");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // revertir a int con identity
            migrationBuilder.AlterColumn<int>(
                name: "IdPago",
                table: "Pagos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()")
                .Annotation("Npgsql:ValueGenerationStrategy",
                    Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
