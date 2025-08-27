using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class agregarTelefono : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Pagos",
                type: "varchar",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Gestiones",
                type: "varchar",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "CompromisosPagos",
                type: "varchar",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Gestiones");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "CompromisosPagos");
        }
    }
}
