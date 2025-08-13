using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbonosLiquidacion",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbonosLiquidacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BancosPagos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BancosPagos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Cedula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    TelefonoContacto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Correo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                });

            migrationBuilder.CreateTable(
                name: "FormasPago",
                columns: table => new
                {
                    FormaPagoId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasPago", x => x.FormaPagoId);
                });

            migrationBuilder.CreateTable(
                name: "TiposContactoGestion",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposContactoGestion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposCuentaBancaria",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCuentaBancaria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposGestion",
                columns: table => new
                {
                    IdTipoGestion = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: true),
                    IdPadre = table.Column<string>(type: "character varying(40)", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "TiposResultado",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposResultado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposTransaccion",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTransaccion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    NombreUsuario = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    rol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Contrasena = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "TiposContactoResultado",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    TipoResultadoId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposContactoResultado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposContactoResultado_TiposResultado_TipoResultadoId",
                        column: x => x.TipoResultadoId,
                        principalTable: "TiposResultado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deudores",
                columns: table => new
                {
                    IdDeudor = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Correo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IdUsuario = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deudores", x => x.IdDeudor);
                    table.ForeignKey(
                        name: "FK_Deudores_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RespuestasTipoContacto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    IdTipoContactoResultado = table.Column<string>(type: "character varying(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestasTipoContacto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespuestasTipoContacto_TiposContactoGestion_Id",
                        column: x => x.Id,
                        principalTable: "TiposContactoGestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RespuestasTipoContacto_TiposContactoResultado_IdTipoContact~",
                        column: x => x.IdTipoContactoResultado,
                        principalTable: "TiposContactoResultado",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsultasExternas",
                columns: table => new
                {
                    IdDeudor = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    JsonRespuesta = table.Column<string>(type: "text", nullable: false),
                    FechaConsulta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IdUsuario = table.Column<string>(type: "character varying(13)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultasExternas", x => x.IdDeudor);
                    table.ForeignKey(
                        name: "FK_ConsultasExternas_Deudores_IdDeudor",
                        column: x => x.IdDeudor,
                        principalTable: "Deudores",
                        principalColumn: "IdDeudor");
                    table.ForeignKey(
                        name: "FK_ConsultasExternas_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Deudas",
                columns: table => new
                {
                    IdDeuda = table.Column<Guid>(type: "uuid", nullable: false),
                    IdDeudor = table.Column<string>(type: "character varying(13)", nullable: true),
                    MontoOriginal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    SaldoActual = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaVencimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaAsignacion = table.Column<DateOnly>(type: "date", nullable: true),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NumeroFactura = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TotalFactura = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    NumeroAutorizacion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SaldoDeuda = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    NumeroCuotas = table.Column<int>(type: "integer", nullable: true),
                    CuotaActual = table.Column<int>(type: "integer", nullable: true),
                    ValorCuota = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Tramo = table.Column<string>(type: "text", nullable: true),
                    UltimoPago = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Empresa = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deudas", x => x.IdDeuda);
                    table.ForeignKey(
                        name: "FK_Deudas_Deudores_IdDeudor",
                        column: x => x.IdDeudor,
                        principalTable: "Deudores",
                        principalColumn: "IdDeudor",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DeudorTelefonos",
                columns: table => new
                {
                    IdDeudorTelefonos = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    IdDeudor = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    FechaAdicion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EsValido = table.Column<bool>(type: "boolean", nullable: true),
                    Origen = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Observacion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeudorTelefonos", x => x.IdDeudorTelefonos);
                    table.ForeignKey(
                        name: "FK_DeudorTelefonos_Deudores_IdDeudor",
                        column: x => x.IdDeudor,
                        principalTable: "Deudores",
                        principalColumn: "IdDeudor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AsignacionesCarteras",
                columns: table => new
                {
                    IdAsignacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdDeuda = table.Column<Guid>(type: "uuid", nullable: false),
                    IdCliente = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    FechaAsignacion = table.Column<DateOnly>(type: "date", nullable: false),
                    FechaRetiro = table.Column<DateOnly>(type: "date", nullable: true),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValue: "activo"),
                    Observaciones = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionesCarteras", x => x.IdAsignacion);
                    table.ForeignKey(
                        name: "FK_AsignacionesCarteras_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente");
                    table.ForeignKey(
                        name: "FK_AsignacionesCarteras_Deudas_IdDeuda",
                        column: x => x.IdDeuda,
                        principalTable: "Deudas",
                        principalColumn: "IdDeuda");
                });

            migrationBuilder.CreateTable(
                name: "CompromisosPagos",
                columns: table => new
                {
                    IdCompromiso = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    IdDeuda = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaCompromiso = table.Column<DateOnly>(type: "date", nullable: false),
                    MontoComprometido = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValue: "pendiente"),
                    FechaCumplimientoReal = table.Column<DateOnly>(type: "date", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    IdUsuario = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompromisosPagos", x => x.IdCompromiso);
                    table.ForeignKey(
                        name: "FK_CompromisosPagos_Deudas_IdDeuda",
                        column: x => x.IdDeuda,
                        principalTable: "Deudas",
                        principalColumn: "IdDeuda",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompromisosPagos_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Gestiones",
                columns: table => new
                {
                    IdGestion = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    IdDeuda = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaGestion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IdTipoGestion = table.Column<string>(type: "character varying(40)", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IdUsuarioGestiona = table.Column<string>(type: "character varying(13)", nullable: false),
                    IdTipoContactoGestion = table.Column<string>(type: "character varying(50)", nullable: false),
                    IdRespuestaTipoContacto = table.Column<string>(type: "character varying(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gestiones", x => x.IdGestion);
                    table.ForeignKey(
                        name: "FK_Gestiones_Deudas_IdDeuda",
                        column: x => x.IdDeuda,
                        principalTable: "Deudas",
                        principalColumn: "IdDeuda",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Gestiones_RespuestasTipoContacto_IdRespuestaTipoContacto",
                        column: x => x.IdRespuestaTipoContacto,
                        principalTable: "RespuestasTipoContacto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gestiones_TiposContactoGestion_IdTipoContactoGestion",
                        column: x => x.IdTipoContactoGestion,
                        principalTable: "TiposContactoGestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gestiones_TiposGestion_IdTipoGestion",
                        column: x => x.IdTipoGestion,
                        principalTable: "TiposGestion",
                        principalColumn: "IdTipoGestion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gestiones_Usuarios_IdUsuarioGestiona",
                        column: x => x.IdUsuarioGestiona,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    IdPago = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdDeuda = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaPago = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "CURRENT_DATE"),
                    MontoPagado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MedioPago = table.Column<string>(type: "text", nullable: true),
                    NumeroDocumenro = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FormaPagoId = table.Column<string>(type: "character varying(50)", nullable: true),
                    IdUsuario = table.Column<string>(type: "character varying(13)", nullable: true),
                    IdBancosPago = table.Column<string>(type: "character varying(50)", nullable: true),
                    IdTipoCuentaBancaria = table.Column<string>(type: "character varying(50)", nullable: true),
                    IdTipoTransaccion = table.Column<string>(type: "character varying(50)", nullable: true),
                    IdAbonoLiquidacion = table.Column<string>(type: "character varying(50)", nullable: true),
                    UsuarioIdUsuario = table.Column<string>(type: "character varying(13)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.IdPago);
                    table.ForeignKey(
                        name: "FK_Pagos_AbonosLiquidacion_IdAbonoLiquidacion",
                        column: x => x.IdAbonoLiquidacion,
                        principalTable: "AbonosLiquidacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_BancosPagos_IdBancosPago",
                        column: x => x.IdBancosPago,
                        principalTable: "BancosPagos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_Deudas_IdDeuda",
                        column: x => x.IdDeuda,
                        principalTable: "Deudas",
                        principalColumn: "IdDeuda",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pagos_FormasPago_FormaPagoId",
                        column: x => x.FormaPagoId,
                        principalTable: "FormasPago",
                        principalColumn: "FormaPagoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_TiposCuentaBancaria_IdTipoCuentaBancaria",
                        column: x => x.IdTipoCuentaBancaria,
                        principalTable: "TiposCuentaBancaria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_TiposTransaccion_IdTipoTransaccion",
                        column: x => x.IdTipoTransaccion,
                        principalTable: "TiposTransaccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagos_Usuarios_UsuarioIdUsuario",
                        column: x => x.UsuarioIdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesCarteras_IdCliente",
                table: "AsignacionesCarteras",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesCarteras_IdDeuda",
                table: "AsignacionesCarteras",
                column: "IdDeuda");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosPagos_IdDeuda",
                table: "CompromisosPagos",
                column: "IdDeuda");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosPagos_IdUsuario",
                table: "CompromisosPagos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasExternas_IdUsuario",
                table: "ConsultasExternas",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Deudas_IdDeudor",
                table: "Deudas",
                column: "IdDeudor");

            migrationBuilder.CreateIndex(
                name: "IX_Deudores_IdUsuario",
                table: "Deudores",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_DeudorTelefonos_IdDeudor",
                table: "DeudorTelefonos",
                column: "IdDeudor");

            migrationBuilder.CreateIndex(
                name: "IX_Gestiones_IdDeuda",
                table: "Gestiones",
                column: "IdDeuda");

            migrationBuilder.CreateIndex(
                name: "IX_Gestiones_IdRespuestaTipoContacto",
                table: "Gestiones",
                column: "IdRespuestaTipoContacto");

            migrationBuilder.CreateIndex(
                name: "IX_Gestiones_IdTipoContactoGestion",
                table: "Gestiones",
                column: "IdTipoContactoGestion");

            migrationBuilder.CreateIndex(
                name: "IX_Gestiones_IdTipoGestion",
                table: "Gestiones",
                column: "IdTipoGestion");

            migrationBuilder.CreateIndex(
                name: "IX_Gestiones_IdUsuarioGestiona",
                table: "Gestiones",
                column: "IdUsuarioGestiona");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_FormaPagoId",
                table: "Pagos",
                column: "FormaPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdAbonoLiquidacion",
                table: "Pagos",
                column: "IdAbonoLiquidacion");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdBancosPago",
                table: "Pagos",
                column: "IdBancosPago");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdDeuda",
                table: "Pagos",
                column: "IdDeuda");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdTipoCuentaBancaria",
                table: "Pagos",
                column: "IdTipoCuentaBancaria");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdTipoTransaccion",
                table: "Pagos",
                column: "IdTipoTransaccion");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_IdUsuario",
                table: "Pagos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_UsuarioIdUsuario",
                table: "Pagos",
                column: "UsuarioIdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasTipoContacto_IdTipoContactoResultado",
                table: "RespuestasTipoContacto",
                column: "IdTipoContactoResultado");

            migrationBuilder.CreateIndex(
                name: "IX_TiposContactoResultado_TipoResultadoId",
                table: "TiposContactoResultado",
                column: "TipoResultadoId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposGestion_IdPadre",
                table: "TiposGestion",
                column: "IdPadre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionesCarteras");

            migrationBuilder.DropTable(
                name: "CompromisosPagos");

            migrationBuilder.DropTable(
                name: "ConsultasExternas");

            migrationBuilder.DropTable(
                name: "DeudorTelefonos");

            migrationBuilder.DropTable(
                name: "Gestiones");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "RespuestasTipoContacto");

            migrationBuilder.DropTable(
                name: "TiposGestion");

            migrationBuilder.DropTable(
                name: "AbonosLiquidacion");

            migrationBuilder.DropTable(
                name: "BancosPagos");

            migrationBuilder.DropTable(
                name: "Deudas");

            migrationBuilder.DropTable(
                name: "FormasPago");

            migrationBuilder.DropTable(
                name: "TiposCuentaBancaria");

            migrationBuilder.DropTable(
                name: "TiposTransaccion");

            migrationBuilder.DropTable(
                name: "TiposContactoGestion");

            migrationBuilder.DropTable(
                name: "TiposContactoResultado");

            migrationBuilder.DropTable(
                name: "Deudores");

            migrationBuilder.DropTable(
                name: "TiposResultado");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
