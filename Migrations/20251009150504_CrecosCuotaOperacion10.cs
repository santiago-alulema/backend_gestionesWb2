using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class CrecosCuotaOperacion10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "COD_OPERACION",
                schema: "temp_crecos",
                table: "CuotasOperacionCrecos",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "IVALORCUOTA",
                schema: "temp_crecos",
                table: "CuotasOperacionCrecos",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VALOR_CAPITAL_INTERES",
                schema: "temp_crecos",
                table: "CuotasOperacionCrecos",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VALOR_CARGOS",
                schema: "temp_crecos",
                table: "CuotasOperacionCrecos",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VALOR_OTROS_CARGOS",
                schema: "temp_crecos",
                table: "CuotasOperacionCrecos",
                type: "numeric(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IVALORCUOTA",
                schema: "temp_crecos",
                table: "CuotasOperacionCrecos");

            migrationBuilder.DropColumn(
                name: "VALOR_CAPITAL_INTERES",
                schema: "temp_crecos",
                table: "CuotasOperacionCrecos");

            migrationBuilder.DropColumn(
                name: "VALOR_CARGOS",
                schema: "temp_crecos",
                table: "CuotasOperacionCrecos");

            migrationBuilder.DropColumn(
                name: "VALOR_OTROS_CARGOS",
                schema: "temp_crecos",
                table: "CuotasOperacionCrecos");

            migrationBuilder.AlterColumn<string>(
                name: "COD_OPERACION",
                schema: "temp_crecos",
                table: "CuotasOperacionCrecos",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);
        }
    }
}
