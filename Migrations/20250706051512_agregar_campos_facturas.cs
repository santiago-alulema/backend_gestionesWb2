using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class agregar_campos_facturas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "tipos_gestion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoComponente",
                table: "tipos_gestion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "numero_autorizacion",
                table: "deudas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "numero_factura",
                table: "deudas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "total_factura",
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
                name: "Estado",
                table: "tipos_gestion");

            migrationBuilder.DropColumn(
                name: "TipoComponente",
                table: "tipos_gestion");

            migrationBuilder.DropColumn(
                name: "numero_autorizacion",
                table: "deudas");

            migrationBuilder.DropColumn(
                name: "numero_factura",
                table: "deudas");

            migrationBuilder.DropColumn(
                name: "total_factura",
                table: "deudas");
        }
    }
}
