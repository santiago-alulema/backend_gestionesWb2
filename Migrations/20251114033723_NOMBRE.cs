using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class NOMBRE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trifocuscrecospartes",
                schema: "temp_crecos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(36)", nullable: false),
                    codoperacion = table.Column<string>(type: "varchar", nullable: true),
                    valorliquidacion = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    valorliquidacionparte = table.Column<decimal>(type: "numeric(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trifocuscrecospartes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trifocuscrecospartes",
                schema: "temp_crecos");
        }
    }
}
