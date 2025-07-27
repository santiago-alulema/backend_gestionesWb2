using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposNuevosADeuda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cuota_actual",
                table: "deudas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "empresa",
                table: "deudas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "numero_cuotas",
                table: "deudas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "saldo_deuda",
                table: "deudas",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tramo",
                table: "deudas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ultimo_pago",
                table: "deudas",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "valor_cuota",
                table: "deudas",
                type: "numeric(12,2)",
                precision: 12,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cuota_actual",
                table: "deudas");

            migrationBuilder.DropColumn(
                name: "empresa",
                table: "deudas");

            migrationBuilder.DropColumn(
                name: "numero_cuotas",
                table: "deudas");

            migrationBuilder.DropColumn(
                name: "saldo_deuda",
                table: "deudas");

            migrationBuilder.DropColumn(
                name: "tramo",
                table: "deudas");

            migrationBuilder.DropColumn(
                name: "ultimo_pago",
                table: "deudas");

            migrationBuilder.DropColumn(
                name: "valor_cuota",
                table: "deudas");
        }
    }
}
