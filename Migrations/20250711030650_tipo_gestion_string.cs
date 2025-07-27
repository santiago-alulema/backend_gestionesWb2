using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class tipo_gestion_string : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tipo_gestion",
                table: "tipos_gestion",
                type: "varchar(20)",
                nullable: false,
                comment: "P Padre H Hijo",
                oldClrType: typeof(string),
                oldType: "character varying(1)",
                oldMaxLength: 1,
                oldDefaultValueSql: "'H'::character varying",
                oldComment: "P Padre H Hijo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tipo_gestion",
                table: "tipos_gestion",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValueSql: "'H'::character varying",
                comment: "P Padre H Hijo",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldComment: "P Padre H Hijo");
        }
    }
}
