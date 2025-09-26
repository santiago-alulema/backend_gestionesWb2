using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioDeudaRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdUsuario",
                table: "Deudas",
                type: "character varying(13)",
                maxLength: 13,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdUsuario",
                table: "Usuarios",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Deudas_IdUsuario",
                table: "Deudas",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Deudas_NumeroFactura",
                table: "Deudas",
                column: "NumeroFactura");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Cedula",
                table: "Clientes",
                column: "Cedula");

            migrationBuilder.AddForeignKey(
                name: "FK_Deudas_Usuarios_IdUsuario",
                table: "Deudas",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deudas_Usuarios_IdUsuario",
                table: "Deudas");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_IdUsuario",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Deudas_IdUsuario",
                table: "Deudas");

            migrationBuilder.DropIndex(
                name: "IX_Deudas_NumeroFactura",
                table: "Deudas");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_Cedula",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Deudas");
        }
    }
}
