using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class tablasTempNombreArchivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nombre_Archivo",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre_Archivo",
                schema: "temp_crecos",
                table: "ReciboFormaPagoCrecos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nombre_Archivo",
                schema: "temp_crecos",
                table: "ReciboDetalleCrecos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre_Archivo",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos");

            migrationBuilder.DropColumn(
                name: "Nombre_Archivo",
                schema: "temp_crecos",
                table: "ReciboFormaPagoCrecos");

            migrationBuilder.DropColumn(
                name: "Nombre_Archivo",
                schema: "temp_crecos",
                table: "ReciboDetalleCrecos");
        }
    }
}
