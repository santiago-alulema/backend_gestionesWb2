using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class agregarNombrearchivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NOMBRE_ARCHIVO",
                schema: "temp_crecos",
                table: "SaldoClienteCrecos",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaldoClienteCrecos_NOMBRE_ARCHIVO",
                schema: "temp_crecos",
                table: "SaldoClienteCrecos",
                column: "NOMBRE_ARCHIVO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SaldoClienteCrecos_NOMBRE_ARCHIVO",
                schema: "temp_crecos",
                table: "SaldoClienteCrecos");

            migrationBuilder.DropColumn(
                name: "NOMBRE_ARCHIVO",
                schema: "temp_crecos",
                table: "SaldoClienteCrecos");
        }
    }
}
