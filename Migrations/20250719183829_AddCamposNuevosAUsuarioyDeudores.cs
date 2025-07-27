using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposNuevosAUsuarioyDeudores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_deudor",
                table: "deudores",
                newName: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "ix_deudores_id_usuario",
                table: "deudores",
                column: "id_usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_deudores_usuarios_id_usuario",
                table: "deudores",
                column: "id_usuario",
                principalTable: "usuarios",
                principalColumn: "id_usuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deudores_usuarios_id_usuario",
                table: "deudores");

            migrationBuilder.DropIndex(
                name: "ix_deudores_id_usuario",
                table: "deudores");

            migrationBuilder.RenameColumn(
                name: "id_usuario",
                table: "deudores",
                newName: "id_deudor");
        }
    }
}
