using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class AumentarCaracteresElIdDeudores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdDeudor",
                table: "Deudores",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(13)",
                oldMaxLength: 13);

            migrationBuilder.AlterColumn<string>(
                name: "IdDeudor",
                table: "Deudas",
                type: "character varying(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(13)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdDeudor",
                table: "Deudores",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<string>(
                name: "IdDeudor",
                table: "Deudas",
                type: "character varying(13)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldNullable: true);
        }
    }
}
