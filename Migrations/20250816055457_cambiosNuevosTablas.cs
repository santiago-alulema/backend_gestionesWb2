using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class cambiosNuevosTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "FechaVencimiento",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "MontoOriginal",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "NumeroAutorizacion",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "SaldoActual",
                table: "Deudas");

            migrationBuilder.RenameColumn(
                name: "TotalFactura",
                table: "Deudas",
                newName: "SaldoDeulda");

            migrationBuilder.RenameColumn(
                name: "FechaAsignacion",
                table: "Deudas",
                newName: "FechaVenta");

            migrationBuilder.RenameColumn(
                name: "CuotaActual",
                table: "Deudas",
                newName: "DiasMora");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroFactura",
                table: "Deudas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Deudas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Empresa",
                table: "Deudas",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Clasificacion",
                table: "Deudas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Creditos",
                table: "Deudas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Descuento",
                table: "Deudas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeudaCapital",
                table: "Deudas",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "FechaUltimoPago",
                table: "Deudas",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GastosCobranzas",
                table: "Deudas",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Interes",
                table: "Deudas",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoCobrar",
                table: "Deudas",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoDocumento",
                table: "Deudas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Clasificacion",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "Creditos",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "Descuento",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "DeudaCapital",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "FechaUltimoPago",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "GastosCobranzas",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "Interes",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "MontoCobrar",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "TipoDocumento",
                table: "Deudas");

            migrationBuilder.RenameColumn(
                name: "SaldoDeulda",
                table: "Deudas",
                newName: "TotalFactura");

            migrationBuilder.RenameColumn(
                name: "FechaVenta",
                table: "Deudas",
                newName: "FechaAsignacion");

            migrationBuilder.RenameColumn(
                name: "DiasMora",
                table: "Deudas",
                newName: "CuotaActual");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroFactura",
                table: "Deudas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Deudas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Empresa",
                table: "Deudas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Deudas",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "FechaVencimiento",
                table: "Deudas",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<decimal>(
                name: "MontoOriginal",
                table: "Deudas",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "NumeroAutorizacion",
                table: "Deudas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoActual",
                table: "Deudas",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
