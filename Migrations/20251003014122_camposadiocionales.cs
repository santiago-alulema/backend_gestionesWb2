using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class camposadiocionales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoUsuario",
                table: "Usuarios",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoEmpresaExterna",
                table: "TiposResultado",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoEmpresaExterna",
                table: "TiposContactoResultado",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoEmpresaExterna",
                table: "TiposContactoGestion",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoEmpresaExterna",
                table: "RespuestasTipoContacto",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodigoDeudor",
                table: "Deudores",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Deudores",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()");

            migrationBuilder.AddColumn<string>(
                name: "CodigoEmpresa",
                table: "Deudas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Deudas",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "now()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoUsuario",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CodigoEmpresaExterna",
                table: "TiposResultado");

            migrationBuilder.DropColumn(
                name: "CodigoEmpresaExterna",
                table: "TiposContactoResultado");

            migrationBuilder.DropColumn(
                name: "CodigoEmpresaExterna",
                table: "TiposContactoGestion");

            migrationBuilder.DropColumn(
                name: "CodigoEmpresaExterna",
                table: "RespuestasTipoContacto");

            migrationBuilder.DropColumn(
                name: "CodigoDeudor",
                table: "Deudores");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Deudores");

            migrationBuilder.DropColumn(
                name: "CodigoEmpresa",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Deudas");
        }
    }
}
