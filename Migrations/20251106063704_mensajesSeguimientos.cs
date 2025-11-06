using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class mensajesSeguimientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeguimientoMensajes",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tipo = table.Column<string>(type: "varchar", nullable: true),
                    numeroDestino = table.Column<string>(type: "varchar", nullable: true),
                    usuario = table.Column<string>(type: "varchar", nullable: true),
                    usuarioWhatsapp = table.Column<string>(type: "varchar", nullable: true),
                    mensaje = table.Column<string>(type: "text", nullable: true),
                    fechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeguimientoMensajes", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeguimientoMensajes");
        }
    }
}
