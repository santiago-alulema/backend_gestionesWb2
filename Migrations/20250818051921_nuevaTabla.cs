using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class nuevaTabla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HoraRecordatorio",
                table: "CompromisosPagos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdTipoTarea",
                table: "CompromisosPagos",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TiposTareas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTareas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosPagos_IdTipoTarea",
                table: "CompromisosPagos",
                column: "IdTipoTarea");

            migrationBuilder.AddForeignKey(
                name: "FK_CompromisosPagos_TiposTareas_IdTipoTarea",
                table: "CompromisosPagos",
                column: "IdTipoTarea",
                principalTable: "TiposTareas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompromisosPagos_TiposTareas_IdTipoTarea",
                table: "CompromisosPagos");

            migrationBuilder.DropTable(
                name: "TiposTareas");

            migrationBuilder.DropIndex(
                name: "IX_CompromisosPagos_IdTipoTarea",
                table: "CompromisosPagos");

            migrationBuilder.DropColumn(
                name: "HoraRecordatorio",
                table: "CompromisosPagos");

            migrationBuilder.DropColumn(
                name: "IdTipoTarea",
                table: "CompromisosPagos");
        }
    }
}
