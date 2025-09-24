using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class MensajesWhatsappPlantilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MensajeWhatsappUsuario_Usuarios_IdUsuario",
                table: "MensajeWhatsappUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MensajeWhatsappUsuario",
                table: "MensajeWhatsappUsuario");

            migrationBuilder.DropIndex(
                name: "IX_MensajeWhatsappUsuario_IdUsuario",
                table: "MensajeWhatsappUsuario");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "MensajeWhatsappUsuario");

            migrationBuilder.RenameTable(
                name: "MensajeWhatsappUsuario",
                newName: "MensajesWhatsapp");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MensajesWhatsapp",
                table: "MensajesWhatsapp",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MensajesWhatsapp",
                table: "MensajesWhatsapp");

            migrationBuilder.RenameTable(
                name: "MensajesWhatsapp",
                newName: "MensajeWhatsappUsuario");

            migrationBuilder.AddColumn<string>(
                name: "IdUsuario",
                table: "MensajeWhatsappUsuario",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MensajeWhatsappUsuario",
                table: "MensajeWhatsappUsuario",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MensajeWhatsappUsuario_IdUsuario",
                table: "MensajeWhatsappUsuario",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_MensajeWhatsappUsuario_Usuarios_IdUsuario",
                table: "MensajeWhatsappUsuario",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
