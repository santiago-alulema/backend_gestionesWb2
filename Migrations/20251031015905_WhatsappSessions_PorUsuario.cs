using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class WhatsappSessions_PorUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdUsuario",
                table: "WhatsappSessions",
                type: "character varying(13)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhatsappSessions_IdUsuario",
                table: "WhatsappSessions",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_WhatsappSessions_Usuarios_IdUsuario",
                table: "WhatsappSessions",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WhatsappSessions_Usuarios_IdUsuario",
                table: "WhatsappSessions");

            migrationBuilder.DropIndex(
                name: "IX_WhatsappSessions_IdUsuario",
                table: "WhatsappSessions");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "WhatsappSessions");
        }
    }
}
