using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class FixTimestampMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gestiones_TiposContactoGestion_IdTipoContactoGestion",
                table: "Gestiones");

            migrationBuilder.DropForeignKey(
                name: "FK_Gestiones_TiposGestion_IdTipoGestion",
                table: "Gestiones");

            migrationBuilder.DropIndex(
                name: "IX_Gestiones_IdTipoGestion",
                table: "Gestiones");

            migrationBuilder.RenameColumn(
                name: "IdTipoContactoGestion",
                table: "Gestiones",
                newName: "IdTipoContactoResultado");

            migrationBuilder.RenameIndex(
                name: "IX_Gestiones_IdTipoContactoGestion",
                table: "Gestiones",
                newName: "IX_Gestiones_IdTipoContactoResultado");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Pagos",
                type: "timestamp without time zone",
                nullable: true,
                defaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<string>(
                name: "IdTipoGestion",
                table: "Gestiones",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Gestiones",
                type: "character varying(900)",
                maxLength: 900,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "IdTipoResultado",
                table: "Gestiones",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdTipoResultadoNavigationId",
                table: "Gestiones",
                type: "character varying(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TiposGestionIdTipoGestion",
                table: "Gestiones",
                type: "character varying(40)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gestiones_IdTipoResultadoNavigationId",
                table: "Gestiones",
                column: "IdTipoResultadoNavigationId");

            migrationBuilder.CreateIndex(
                name: "IX_Gestiones_TiposGestionIdTipoGestion",
                table: "Gestiones",
                column: "TiposGestionIdTipoGestion");

            migrationBuilder.AddForeignKey(
                name: "FK_Gestiones_TiposContactoResultado_IdTipoContactoResultado",
                table: "Gestiones",
                column: "IdTipoContactoResultado",
                principalTable: "TiposContactoResultado",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gestiones_TiposGestion_TiposGestionIdTipoGestion",
                table: "Gestiones",
                column: "TiposGestionIdTipoGestion",
                principalTable: "TiposGestion",
                principalColumn: "IdTipoGestion");

            migrationBuilder.AddForeignKey(
                name: "FK_Gestiones_TiposResultado_IdTipoResultadoNavigationId",
                table: "Gestiones",
                column: "IdTipoResultadoNavigationId",
                principalTable: "TiposResultado",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gestiones_TiposContactoResultado_IdTipoContactoResultado",
                table: "Gestiones");

            migrationBuilder.DropForeignKey(
                name: "FK_Gestiones_TiposGestion_TiposGestionIdTipoGestion",
                table: "Gestiones");

            migrationBuilder.DropForeignKey(
                name: "FK_Gestiones_TiposResultado_IdTipoResultadoNavigationId",
                table: "Gestiones");

            migrationBuilder.DropIndex(
                name: "IX_Gestiones_IdTipoResultadoNavigationId",
                table: "Gestiones");

            migrationBuilder.DropIndex(
                name: "IX_Gestiones_TiposGestionIdTipoGestion",
                table: "Gestiones");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "IdTipoResultado",
                table: "Gestiones");

            migrationBuilder.DropColumn(
                name: "IdTipoResultadoNavigationId",
                table: "Gestiones");

            migrationBuilder.DropColumn(
                name: "TiposGestionIdTipoGestion",
                table: "Gestiones");

            migrationBuilder.RenameColumn(
                name: "IdTipoContactoResultado",
                table: "Gestiones",
                newName: "IdTipoContactoGestion");

            migrationBuilder.RenameIndex(
                name: "IX_Gestiones_IdTipoContactoResultado",
                table: "Gestiones",
                newName: "IX_Gestiones_IdTipoContactoGestion");

            migrationBuilder.AlterColumn<string>(
                name: "IdTipoGestion",
                table: "Gestiones",
                type: "character varying(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Gestiones",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(900)",
                oldMaxLength: 900);

            migrationBuilder.CreateIndex(
                name: "IX_Gestiones_IdTipoGestion",
                table: "Gestiones",
                column: "IdTipoGestion");

            migrationBuilder.AddForeignKey(
                name: "FK_Gestiones_TiposContactoGestion_IdTipoContactoGestion",
                table: "Gestiones",
                column: "IdTipoContactoGestion",
                principalTable: "TiposContactoGestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gestiones_TiposGestion_IdTipoGestion",
                table: "Gestiones",
                column: "IdTipoGestion",
                principalTable: "TiposGestion",
                principalColumn: "IdTipoGestion",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
