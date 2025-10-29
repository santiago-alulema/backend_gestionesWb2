using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class tablasTempId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReciboPagosCrecos",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos");

            migrationBuilder.DropIndex(
                name: "IX_ReciboPagos_FechaPago",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos");

            migrationBuilder.DropIndex(
                name: "IX_ReciboPagos_NumIdentificacion",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReciboFormaPagoCrecos",
                schema: "temp_crecos",
                table: "ReciboFormaPagoCrecos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReciboDetalleCrecos",
                schema: "temp_crecos",
                table: "ReciboDetalleCrecos");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                schema: "temp_crecos",
                table: "ReciboFormaPagoCrecos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                schema: "temp_crecos",
                table: "ReciboDetalleCrecos",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdReciboPagosCrecos",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdReciboFormaPagoCrecos",
                schema: "temp_crecos",
                table: "ReciboFormaPagoCrecos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdReciboDetalleCrecos",
                schema: "temp_crecos",
                table: "ReciboDetalleCrecos",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_IdReciboPagosCrecos",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdReciboFormaPagoCrecos",
                schema: "temp_crecos",
                table: "ReciboFormaPagoCrecos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdReciboDetalleCrecos",
                schema: "temp_crecos",
                table: "ReciboDetalleCrecos");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "temp_crecos",
                table: "ReciboFormaPagoCrecos");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "temp_crecos",
                table: "ReciboDetalleCrecos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReciboPagosCrecos",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos",
                column: "COD_RECIBO");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReciboFormaPagoCrecos",
                schema: "temp_crecos",
                table: "ReciboFormaPagoCrecos",
                column: "COD_RECIBO_FORMAPAGO");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReciboDetalleCrecos",
                schema: "temp_crecos",
                table: "ReciboDetalleCrecos",
                column: "IRECIBODETALLE");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboPagos_FechaPago",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos",
                column: "FECHA_PAGO");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboPagos_NumIdentificacion",
                schema: "temp_crecos",
                table: "ReciboPagosCrecos",
                column: "NUM_IDENTIFICACION");
        }
    }
}
