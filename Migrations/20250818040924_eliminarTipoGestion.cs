using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class eliminarTipoGestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gestiones_TiposGestion_TiposGestionIdTipoGestion",
                table: "Gestiones");

            migrationBuilder.DropTable(
                name: "TiposGestion");

            migrationBuilder.DropIndex(
                name: "IX_Gestiones_TiposGestionIdTipoGestion",
                table: "Gestiones");

            migrationBuilder.DropColumn(
                name: "IdTipoGestion",
                table: "Gestiones");

            migrationBuilder.DropColumn(
                name: "TiposGestionIdTipoGestion",
                table: "Gestiones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdTipoGestion",
                table: "Gestiones",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TiposGestionIdTipoGestion",
                table: "Gestiones",
                type: "character varying(40)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TiposGestion",
                columns: table => new
                {
                    IdTipoGestion = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    IdPadre = table.Column<string>(type: "character varying(40)", nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: true),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TipoGestion = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposGestion", x => x.IdTipoGestion);
                    table.ForeignKey(
                        name: "FK_TiposGestion_TiposGestion_IdPadre",
                        column: x => x.IdPadre,
                        principalTable: "TiposGestion",
                        principalColumn: "IdTipoGestion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gestiones_TiposGestionIdTipoGestion",
                table: "Gestiones",
                column: "TiposGestionIdTipoGestion");

            migrationBuilder.CreateIndex(
                name: "IX_TiposGestion_IdPadre",
                table: "TiposGestion",
                column: "IdPadre");

            migrationBuilder.AddForeignKey(
                name: "FK_Gestiones_TiposGestion_TiposGestionIdTipoGestion",
                table: "Gestiones",
                column: "TiposGestionIdTipoGestion",
                principalTable: "TiposGestion",
                principalColumn: "IdTipoGestion");
        }
    }
}
