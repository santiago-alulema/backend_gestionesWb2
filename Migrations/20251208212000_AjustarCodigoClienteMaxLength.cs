using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class AjustarCodigoClienteMaxLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SaldoClienteCrecos_NOMBRE_ARCHIVO",
                schema: "temp_crecos",
                table: "SaldoClienteCrecos");

            migrationBuilder.AlterColumn<string>(
                name: "NOMBRE_ARCHIVO",
                schema: "temp_crecos",
                table: "SaldoClienteCrecos",
                type: "varchar",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NOMBRE_ARCHIVO",
                schema: "temp_crecos",
                table: "SaldoClienteCrecos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaldoClienteCrecos_NOMBRE_ARCHIVO",
                schema: "temp_crecos",
                table: "SaldoClienteCrecos",
                column: "NOMBRE_ARCHIVO");
        }
    }
}
