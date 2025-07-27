using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class nueva_migracion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE deudor_telefonos ALTER COLUMN id_deudor_telefonos DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql("ALTER TABLE deudor_telefonos ALTER COLUMN id_deudor_telefonos TYPE varchar(40);");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "id_deudor_telefonos",
                table: "deudor_telefonos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
