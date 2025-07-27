using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id_cliente = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    direccion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    telefono_contacto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    correo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("clientes_pkey", x => x.id_cliente);
                });

            migrationBuilder.CreateTable(
                name: "deudores",
                columns: table => new
                {
                    id_deudor = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    direccion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    correo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("deudores_pkey", x => x.id_deudor);
                });

            migrationBuilder.CreateTable(
                name: "tipos_gestion",
                columns: table => new
                {
                    id_tipo_gestion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    id_padre = table.Column<int>(type: "integer", nullable: true),
                    tipo_gestion = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false, defaultValueSql: "'H'::character varying", comment: "P Padre H Hijo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tipos_gestion_pkey", x => x.id_tipo_gestion);
                    table.ForeignKey(
                        name: "tipos_gestion_id_padre_fkey",
                        column: x => x.id_padre,
                        principalTable: "tipos_gestion",
                        principalColumn: "id_tipo_gestion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    rol = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false, defaultValueSql: "'U'::character varying", comment: "U usuario A Admin"),
                    contrasena = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usuarios_pkey", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "deudas",
                columns: table => new
                {
                    id_deuda = table.Column<Guid>(type: "uuid", maxLength: 36, nullable: false, defaultValueSql: "gen_random_uuid()"),
                    id_deudor = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    monto_original = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    saldo_actual = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    fecha_vencimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_asignacion = table.Column<DateOnly>(type: "date", nullable: true),
                    estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    descripcion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("deudas_pkey", x => x.id_deuda);
                    table.ForeignKey(
                        name: "deudas_deudores_fk",
                        column: x => x.id_deudor,
                        principalTable: "deudores",
                        principalColumn: "id_deudor");
                });

            migrationBuilder.CreateTable(
                name: "deudor_telefonos",
                columns: table => new
                {
                    id_deudor_telefonos = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_deudor = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    telefono = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    fecha_adicion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    es_valido = table.Column<bool>(type: "boolean", nullable: true),
                    origen = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("deudor_telefonos_pk", x => x.id_deudor_telefonos);
                    table.ForeignKey(
                        name: "deudor_telefonos_deudores_fk",
                        column: x => x.id_deudor,
                        principalTable: "deudores",
                        principalColumn: "id_deudor");
                });

            migrationBuilder.CreateTable(
                name: "consultas_externas",
                columns: table => new
                {
                    id_deudor = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    json_respuesta = table.Column<string>(type: "json", nullable: false),
                    fecha_consulta = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    id_usuario = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "consutlas_externas_deudores_fk",
                        column: x => x.id_deudor,
                        principalTable: "deudores",
                        principalColumn: "id_deudor");
                    table.ForeignKey(
                        name: "consutlas_externas_usuarios_fk",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "asignaciones_cartera",
                columns: table => new
                {
                    id_asignacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_deuda = table.Column<Guid>(type: "uuid", maxLength: 36, nullable: false),
                    id_cliente = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    fecha_asignacion = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_retiro = table.Column<DateOnly>(type: "date", nullable: true),
                    estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'activo'::character varying"),
                    observaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("asignaciones_cartera_pkey", x => x.id_asignacion);
                    table.ForeignKey(
                        name: "asignaciones_cartera_clientes_fk",
                        column: x => x.id_cliente,
                        principalTable: "clientes",
                        principalColumn: "id_cliente");
                    table.ForeignKey(
                        name: "asignaciones_cartera_deudas_fk",
                        column: x => x.id_deuda,
                        principalTable: "deudas",
                        principalColumn: "id_deuda");
                });

            migrationBuilder.CreateTable(
                name: "compromisos_pago",
                columns: table => new
                {
                    id_compromiso = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_deuda = table.Column<Guid>(type: "uuid", maxLength: 36, nullable: true),
                    fecha_compromiso = table.Column<DateOnly>(type: "date", nullable: false),
                    monto_comprometido = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'pendiente'::character varying"),
                    fecha_cumplimiento_real = table.Column<DateOnly>(type: "date", nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true),
                    id_usuario = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("compromisos_pago_pkey", x => x.id_compromiso);
                    table.ForeignKey(
                        name: "compromisos_pago_deudas_fk",
                        column: x => x.id_deuda,
                        principalTable: "deudas",
                        principalColumn: "id_deuda");
                    table.ForeignKey(
                        name: "compromisos_pago_usuarios_fk",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "gestiones",
                columns: table => new
                {
                    id_gestion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_deuda = table.Column<Guid>(type: "uuid", maxLength: 36, nullable: false),
                    fecha_gestion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    id_tipo_gestion = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    id_usuario_gestiona = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("gestiones_pkey", x => x.id_gestion);
                    table.ForeignKey(
                        name: "gestiones_deudas_fk",
                        column: x => x.id_deuda,
                        principalTable: "deudas",
                        principalColumn: "id_deuda");
                    table.ForeignKey(
                        name: "gestiones_id_tipo_gestion_fkey",
                        column: x => x.id_tipo_gestion,
                        principalTable: "tipos_gestion",
                        principalColumn: "id_tipo_gestion");
                    table.ForeignKey(
                        name: "gestiones_usuarios_fk",
                        column: x => x.id_usuario_gestiona,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "pagos",
                columns: table => new
                {
                    id_pago = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_deuda = table.Column<Guid>(type: "uuid", maxLength: 36, nullable: false),
                    fecha_pago = table.Column<DateOnly>(type: "date", nullable: true),
                    monto_pagado = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    medio_pago = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    observaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pagos_pkey", x => x.id_pago);
                    table.ForeignKey(
                        name: "pagos_deudas_fk",
                        column: x => x.id_deuda,
                        principalTable: "deudas",
                        principalColumn: "id_deuda");
                });

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_cartera_id_cliente",
                table: "asignaciones_cartera",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_cartera_id_deuda",
                table: "asignaciones_cartera",
                column: "id_deuda");

            migrationBuilder.CreateIndex(
                name: "IX_compromisos_pago_id_deuda",
                table: "compromisos_pago",
                column: "id_deuda");

            migrationBuilder.CreateIndex(
                name: "IX_compromisos_pago_id_usuario",
                table: "compromisos_pago",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_consultas_externas_id_deudor",
                table: "consultas_externas",
                column: "id_deudor");

            migrationBuilder.CreateIndex(
                name: "IX_consultas_externas_id_usuario",
                table: "consultas_externas",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_deudas_id_deudor",
                table: "deudas",
                column: "id_deudor");

            migrationBuilder.CreateIndex(
                name: "deudor_telefonos_unique",
                table: "deudor_telefonos",
                columns: new[] { "id_deudor", "telefono" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gestiones_id_deuda",
                table: "gestiones",
                column: "id_deuda");

            migrationBuilder.CreateIndex(
                name: "IX_gestiones_id_tipo_gestion",
                table: "gestiones",
                column: "id_tipo_gestion");

            migrationBuilder.CreateIndex(
                name: "IX_gestiones_id_usuario_gestiona",
                table: "gestiones",
                column: "id_usuario_gestiona");

            migrationBuilder.CreateIndex(
                name: "IX_pagos_id_deuda",
                table: "pagos",
                column: "id_deuda");

            migrationBuilder.CreateIndex(
                name: "IX_tipos_gestion_id_padre",
                table: "tipos_gestion",
                column: "id_padre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asignaciones_cartera");

            migrationBuilder.DropTable(
                name: "compromisos_pago");

            migrationBuilder.DropTable(
                name: "consultas_externas");

            migrationBuilder.DropTable(
                name: "deudor_telefonos");

            migrationBuilder.DropTable(
                name: "gestiones");

            migrationBuilder.DropTable(
                name: "pagos");

            migrationBuilder.DropTable(
                name: "clientes");

            migrationBuilder.DropTable(
                name: "tipos_gestion");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "deudas");

            migrationBuilder.DropTable(
                name: "deudores");
        }
    }
}
