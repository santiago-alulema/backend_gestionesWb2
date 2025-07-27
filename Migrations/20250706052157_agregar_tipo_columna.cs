using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class agregar_tipo_columna : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "tipos_gestion",
                newName: "estado");

            migrationBuilder.RenameColumn(
                name: "TipoComponente",
                table: "tipos_gestion",
                newName: "tipo_componente");

            migrationBuilder.Sql(@"
                                  ALTER TABLE tipos_gestion 
                                  ALTER COLUMN estado 
                                  TYPE boolean 
                                  USING estado::boolean;
                                ");

            migrationBuilder.AlterColumn<string>(
                name: "tipo_componente",
                table: "tipos_gestion",
                type: "varchar(25)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "estado",
                table: "tipos_gestion",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "tipo_componente",
                table: "tipos_gestion",
                newName: "TipoComponente");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "tipos_gestion",
                type: "text",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoComponente",
                table: "tipos_gestion",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(25)",
                oldNullable: true);
        }
    }
}
