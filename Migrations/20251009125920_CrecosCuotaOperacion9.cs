using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class CrecosCuotaOperacion9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CuotasOperacionCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar", nullable: false),
                    COD_OPERACION = table.Column<string>(type: "text", nullable: false),
                    COD_CUOTA = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    NUMERO_CUOTA = table.Column<int>(type: "integer", nullable: false),
                    FECHA_VENCIMIENTO = table.Column<DateTime>(type: "date", nullable: true),
                    FECHA_CORTE = table.Column<DateTime>(type: "date", nullable: true),
                    FECHA_ULTIMO_PAGO = table.Column<DateTime>(type: "date", nullable: true),
                    DFECHAPOSTERGACION = table.Column<DateTime>(type: "date", nullable: true),
                    COD_ESTADO_CUOTA = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DESC_ESTADO_OPERACION = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    COD_ESTADO_REGISTRO = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DES_ESTADO_REGISTRO = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    TASA_MORA = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    IVALORTOTALCUOTA = table.Column<decimal>(type: "numeric(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuotasOperacionCrecos", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CuotasOperacion_CodOperacion",
                schema: "temp_crecos",
                table: "CuotasOperacionCrecos",
                column: "COD_OPERACION");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CuotasOperacionCrecos",
                schema: "temp_crecos");
        }
    }
}
