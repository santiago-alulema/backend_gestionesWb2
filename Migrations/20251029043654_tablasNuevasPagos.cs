using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class tablasNuevasPagos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReciboDetalleCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    IRECIBODETALLE = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    COD_RECIBO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ICODIGOOPERACION = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NUM_CUOTA = table.Column<int>(type: "integer", nullable: true),
                    COD_RUBRO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CDESCRIPCION_RUBRO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    VALOR_RECIBO = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReciboDetalleCrecos", x => x.IRECIBODETALLE);
                });

            migrationBuilder.CreateTable(
                name: "ReciboFormaPagoCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    COD_RECIBO_FORMAPAGO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    COD_RECIBO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    COD_FORMA_PAGO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIPC_FPAGO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_INS_FINANCIERA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CDESCRIPCION_INSTITUCION_FINANCIERA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NUM_CUENTA = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NUM_DOCUMENTO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CNOMBRECUENTACORRENTISTA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CCEDULACUENTACORRENTISTA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DFECHACOBRODOCUMENTO = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    COD_MONEDA = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DESCRIPC_MONEDA = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    COD_MOTIVO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIPC_MOTIVO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IVALOR = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReciboFormaPagoCrecos", x => x.COD_RECIBO_FORMAPAGO);
                });

            migrationBuilder.CreateTable(
                name: "ReciboPagosCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    COD_RECIBO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CESTADO_REGISTRO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    COD_EMPRESA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIPC_EMPRESA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_UNEGOCIO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIPC_UNEGOCIO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_TCARTERA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIPC_TCARTERA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_OFICINA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CDESCRIPCION_OFICINA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NUM_IDENTIFICACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    COD_PAGO_REFERENCIAL = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    COD_MONEDA = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DESCRIPC_MONEDA = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    COD_TPAGO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIPC_TPAGO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_CAJA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIPC_CAJA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_GESTOR = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DESCRIPC_GESTOR = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_TRECIBO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIPC_TRECIBO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FECHA_PAGO = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DFECHAREVERSO = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MONTO = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    CAMBIO = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReciboPagosCrecos", x => x.COD_RECIBO);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReciboDetalle_CodRecibo",
                schema: "temp_crecos",
                table: "ReciboDetalleCrecos",
                column: "COD_RECIBO");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboFormaPago_CodRecibo",
                schema: "temp_crecos",
                table: "ReciboFormaPagoCrecos",
                column: "COD_RECIBO");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReciboDetalleCrecos",
                schema: "temp_crecos");

            migrationBuilder.DropTable(
                name: "ReciboFormaPagoCrecos",
                schema: "temp_crecos");

            migrationBuilder.DropTable(
                name: "ReciboPagosCrecos",
                schema: "temp_crecos");
        }
    }
}
