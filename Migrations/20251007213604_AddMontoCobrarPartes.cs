using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddMontoCobrarPartes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "temp_crecos");

           

            migrationBuilder.CreateTable(
                name: "ArticuloOperacionCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ISECUENCIAL = table.Column<int>(type: "integer", nullable: true),
                    COD_PRODUCTO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    COD_OPERACION = table.Column<string>(type: "varchar", nullable: true),
                    DESC_PRODUCTO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CANTIDAD = table.Column<int>(type: "integer", nullable: true),
                    OBSERVACION = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticuloOperacionCrecos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarteraAsignadaCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    COD_EMPRESA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EMPRESA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_UNIDAD_NEGOCIO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UNIDAD_NEGOCIO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_TIPO_CARTERA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TIPO_CARTERA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IMES = table.Column<int>(type: "integer", nullable: true),
                    IANO = table.Column<int>(type: "integer", nullable: true),
                    CNUMEROIDENTIFICACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CNOMBRECOMPLETO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_TIPO_GESTOR = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CDESCRIPCION = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BCUOTAIMPAGA = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DIAS_MORA = table.Column<int>(type: "integer", nullable: true),
                    DFECHAVENCIMIENTO = table.Column<string>(type: "text", nullable: true),
                    IVALORDEUDATOTAL = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ICICLOCORTE = table.Column<int>(type: "integer", nullable: true),
                    COD_PAIS = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PAIS = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_PROVINCIA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PROVINCIA = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_CANTON = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CANTON = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_ZONA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ZONA = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_BARRIO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BARRIO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_GESTOR = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GESTOR = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CODIGOCLIENTE = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarteraAsignadaCrecos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DatosClienteCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CCODIGOTIPOPERSONA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TIPO_PERSONA = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    COD_TIPO_IDENTIF = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_TIPO_IDENTIF = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CNUMEROIDENTIFICACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CNOMBRECOMPLETO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CEXENTOIMPUESTO = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CPRIMERNOMBRE = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CSEGUNDONOMBRE = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CAPELLIDOPATERNO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CAPELLIDOMATERNO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CCODIGOESTADOCIVIL = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_ESTADO_CIVIL = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CCODIGOSEXO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_SEXO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CCODIGOPAIS = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_PAIS = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CCODIGOCIUDADNACIMIENTO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_CIUDAD_NACIMIENTO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CCODIGONACIONALIDAD = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_NACIONALIDAD = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DFECHANACIMIENTO = table.Column<string>(type: "text", nullable: true),
                    INUMEROCARGA = table.Column<int>(type: "integer", nullable: true),
                    CSEPARACIONBIEN = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CCODIGONIVELEDUCACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_NIVEL_EDUC = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CCODIGOTITULO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_TITULO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CRAZONCOMERCIAL = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CNOMBREEMPRESA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CCODIGOCARGO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_CARGO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DFECHAINGRESOLAB = table.Column<string>(type: "text", nullable: true),
                    IINGRESO = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    IEGRESO = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ICODIGOCLIENTE = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ISCORE = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosClienteCrecos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DireccionClienteCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CNUMEROIDENTIFICACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CODIGO_UBICACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_UBICACION = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_PAIS = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_PAIS = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_PROVINCIA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_PROVINCIA = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_CANTON = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_CANTON = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_PARROQUIA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_PARROQUIA = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_BARRIO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_BARRIO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CDIRECCIONCARGA = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    COBSERVACION = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CDIRECCIONCOMPLETA = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CCASILLA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CCORREOELECTRONICO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_TIPO_ESPACIO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_TIPO_ESPACIO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    INUMEROESPACIO = table.Column<int>(type: "integer", nullable: true),
                    INUMEROSOLAR = table.Column<int>(type: "integer", nullable: true),
                    CCALLEPRINCIPAL = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CNUMEROCALLE = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CALLE_SECUND = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CALLE_SECUND_2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CNUMERO_SOLAR = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    COD_TIPO_NUMERACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_TIPO_NUMERACION = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_INDICADOR_POSICION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_IND_POSICION = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    NOMBRE_EDIFICIO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CNUMEROPISO = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PISO_BLOQUE = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    COFICINA_DEPARTAMENTO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    INDICADOR_PRINCIPAL = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    COD_T_PROPIEDAD = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_TIPO_PROPIEDAD = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    AÑO_ANTIGUEDAD = table.Column<int>(type: "integer", nullable: true),
                    MES_ANTIGUEDAD = table.Column<int>(type: "integer", nullable: true),
                    DIAS_ANTIGUEDAD = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DireccionClienteCrecos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperacionesClientesCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ICODIGOOPERACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    COD_OFICINA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESC_OFICINA = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    N_IDENTIFICACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NUM_FACTURA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    COD_MONEDA = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DESC_MONEDA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    COD_PROD_FINANCIERO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DES_PROD_FINANCIERO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ICODIGO_OPERACION_NEGOCIACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NUM_CUOTAS = table.Column<int>(type: "integer", nullable: true),
                    TASA_INTERES = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    FECHA_FACTURA = table.Column<string>(type: "text", nullable: true),
                    FECHA_ULTIMO_VENCIMIENTO = table.Column<string>(type: "text", nullable: true),
                    FECHA_ULTMO_PAGO = table.Column<string>(type: "text", nullable: true),
                    MONTO_CREDITO = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    VALOR_FINANCIAR = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    NUMERO_SOLICITUD = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    COD_T_OPERACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESC_T_OPERACION = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_T_CREDITO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESC_T_CREDITO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_ESTADO_OPERACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESC_ESTADO_OPERACION = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    SECUENC_CUPO = table.Column<int>(type: "integer", nullable: true),
                    ESTADO_REGISTRO = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DES_ESTADO_REGISTRO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    COD_VENDEDOR = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESC_VENDEDOR = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperacionesClientesCrecos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferenciasPersonalesCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NUM_IDENTIFICACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CNOMBRECOMPLETO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_TIPO_IDENT_REF = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIPC_TIPO_IDENTIFIC = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    NUM_IDENTIFIC_REF = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    NOMBRE_REFERENCIA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_PAIS_REF = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_PAIS = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_PROVINCIA_REF = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_PROVINCIA = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_CANTON_REF = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_CANTON = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_TIPO_VINCULO_REF = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_VINCULO = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DIRECCION_REF = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    NUMERO_REFERENCIA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenciasPersonalesCrecos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaldoClienteCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    COD_EMPRESA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCP_EMPRESA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_U_NEGOCIO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESC_U_NEGOCIO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_CARTERA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_CARTERA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_GESTOR = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESC_GESTOR = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IMES = table.Column<int>(type: "integer", nullable: true),
                    IANO = table.Column<int>(type: "integer", nullable: true),
                    COD_OFICINA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CDESCRIPCION_OFICINA = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    COD_TCREDITO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_TCREDITO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CNUMEROIDENTIFICACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    COD_OPERACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CNUMEROTARJETA = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CICLO_CORTE = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESC_CICLOCORTE = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DIAS_VENCIDOS = table.Column<int>(type: "integer", nullable: true),
                    ITRAMO = table.Column<int>(type: "integer", nullable: true),
                    CDESCRIPCIONTRAMO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FECHA_MAX_PAGO = table.Column<string>(type: "text", nullable: true),
                    VALOR_DEUDA = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    VALOR_PAGO_MINIMO = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    VALOR_CORRIENTE = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    VALOR_VENCIDO = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    VALOR_POR_VENCER = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    VALOR_MORA = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    VALOR_GESTION = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    VALOR_VENCIDO_CORTEANTERIOR = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PRIMERA_CUOTA_VENCIDA = table.Column<string>(type: "text", nullable: true),
                    NEGOCIACION_ACTIVA = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DFECHAEJECUCION = table.Column<string>(type: "text", nullable: true),
                    FECHA_INGRESO = table.Column<string>(type: "text", nullable: true),
                    CALIFICACION_CLIENTE = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    F_ULTIMO_CORTE = table.Column<string>(type: "text", nullable: true),
                    FECHA_ULT_PAGO = table.Column<string>(type: "text", nullable: true),
                    VAL_ULT_PAGO = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    VALOR_PAGO_MINIMO_ACTUALIZADO = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    CODIGOCLIENTE = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaldoClienteCrecos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelefonosClienteCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ISECUENCIA = table.Column<int>(type: "integer", nullable: false),
                    CNUMEROIDENTIFICACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    COD_UBICACION = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DESCRIP_UBICACION = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    COD_TIPO_TELEFONO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TIPO_TELEFONO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CNUMERO = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CPREFIJO = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelefonosClienteCrecos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrifocusCrecos",
                schema: "temp_crecos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioGenera = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    GestorAsignado = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Ciudad = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    CodigoCliente = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    IdentificacionCliente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NombreCliente = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EstadoCivil = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    TipoCredito = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    TramoActual = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Formalidad = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    Calificacion = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    NoCorte = table.Column<int>(type: "integer", nullable: true),
                    Semaforo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Operacion = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DeudaRefinanciada = table.Column<bool>(type: "boolean", nullable: true),
                    FechaCompromisodePago = table.Column<DateOnly>(type: "date", nullable: true),
                    DiasVencidosIniciodeMes = table.Column<int>(type: "integer", nullable: true),
                    TRAMO = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    DiasVencidosActuales = table.Column<int>(type: "integer", nullable: true),
                    DeudaTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PagoMinimo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    SaldoVencido = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    FechaUltimoPago = table.Column<DateOnly>(type: "date", nullable: true),
                    MontoUltimoPagado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    TelefonoDomicilio = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CelularDomicilio = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DireccionDomicilio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TelefonoTrabajo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CelularTrabajo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Cargo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    NombreTrabajo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DireccionTrabajo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TelefonoNegocio = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CelularNegocio = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DireccionNegocio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Referencia1 = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TelefonoReferencia1 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Referencia2 = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TelefonoReferencia2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    MontoCarteraAsignada = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    MontoCobrado = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    PoseeVehiculo = table.Column<bool>(type: "boolean", nullable: true),
                    ultimaGestionTerrena = table.Column<DateOnly>(type: "date", nullable: true),
                    gestionTerrenaAnterior = table.Column<DateOnly>(type: "date", nullable: true),
                    ultimaGestionTelefonica = table.Column<DateOnly>(type: "date", nullable: true),
                    gestionTelefonicaAnterior = table.Column<DateOnly>(type: "date", nullable: true),
                    NoGestiones = table.Column<int>(type: "integer", nullable: true),
                    NoCuotasPagadas = table.Column<int>(type: "integer", nullable: true),
                    Valliquidacion = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    Valliquidacionpartes = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrifocusCrecos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticuloOperacionCrecos_COD_OPERACION",
                schema: "temp_crecos",
                table: "ArticuloOperacionCrecos",
                column: "COD_OPERACION");

            migrationBuilder.CreateIndex(
                name: "IX_CarteraAsignadaCrecos_CNUMEROIDENTIFICACION",
                schema: "temp_crecos",
                table: "CarteraAsignadaCrecos",
                column: "CNUMEROIDENTIFICACION");

            migrationBuilder.CreateIndex(
                name: "IX_CarteraAsignadaCrecos_CODIGOCLIENTE",
                schema: "temp_crecos",
                table: "CarteraAsignadaCrecos",
                column: "CODIGOCLIENTE");

            migrationBuilder.CreateIndex(
                name: "IX_CarteraAsignadaCrecos_IMES_IANO",
                schema: "temp_crecos",
                table: "CarteraAsignadaCrecos",
                columns: new[] { "IMES", "IANO" });

            migrationBuilder.CreateIndex(
                name: "IX_DatosClienteCrecos_CNUMEROIDENTIFICACION",
                schema: "temp_crecos",
                table: "DatosClienteCrecos",
                column: "CNUMEROIDENTIFICACION");

            migrationBuilder.CreateIndex(
                name: "IX_OperacionesClientesCrecos_N_IDENTIFICACION",
                schema: "temp_crecos",
                table: "OperacionesClientesCrecos",
                column: "N_IDENTIFICACION");

            migrationBuilder.CreateIndex(
                name: "IX_SaldoClienteCrecos_CNUMEROIDENTIFICACION",
                schema: "temp_crecos",
                table: "SaldoClienteCrecos",
                column: "CNUMEROIDENTIFICACION");

            migrationBuilder.CreateIndex(
                name: "IX_TelefonosClienteCrecos_CNUMEROIDENTIFICACION",
                schema: "temp_crecos",
                table: "TelefonosClienteCrecos",
                column: "CNUMEROIDENTIFICACION");

            migrationBuilder.CreateIndex(
                name: "ix_operacion_cliente",
                schema: "temp_crecos",
                table: "TrifocusCrecos",
                columns: new[] { "Operacion", "CodigoCliente" });

            migrationBuilder.CreateIndex(
                name: "IX_TrifocusCrecos_CodigoCliente",
                schema: "temp_crecos",
                table: "TrifocusCrecos",
                column: "CodigoCliente");

            migrationBuilder.CreateIndex(
                name: "IX_TrifocusCrecos_IdentificacionCliente",
                schema: "temp_crecos",
                table: "TrifocusCrecos",
                column: "IdentificacionCliente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticuloOperacionCrecos",
                schema: "temp_crecos");

            migrationBuilder.DropTable(
                name: "CarteraAsignadaCrecos",
                schema: "temp_crecos");

            migrationBuilder.DropTable(
                name: "DatosClienteCrecos",
                schema: "temp_crecos");

            migrationBuilder.DropTable(
                name: "DireccionClienteCrecos",
                schema: "temp_crecos");

            migrationBuilder.DropTable(
                name: "OperacionesClientesCrecos",
                schema: "temp_crecos");

            migrationBuilder.DropTable(
                name: "ReferenciasPersonalesCrecos",
                schema: "temp_crecos");

            migrationBuilder.DropTable(
                name: "SaldoClienteCrecos",
                schema: "temp_crecos");

            migrationBuilder.DropTable(
                name: "TelefonosClienteCrecos",
                schema: "temp_crecos");

            migrationBuilder.DropTable(
                name: "TrifocusCrecos",
                schema: "temp_crecos");

        }
    }
}
