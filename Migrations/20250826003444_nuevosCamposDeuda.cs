using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class nuevosCamposDeuda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Agencia",
                table: "Deudas",
                type: "varchar",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ciudad",
                table: "Deudas",
                type: "varchar",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "productoDescripcion",
                table: "Deudas",
                type: "varchar",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Agencia",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "Ciudad",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "productoDescripcion",
                table: "Deudas");
        }
    }
}
