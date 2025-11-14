using DocumentFormat.OpenXml.Vml.Office;
using gestiones_backend.Entity;
using gestiones_backend.Entity.temp_crecos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gestiones_backend.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        // DbSets para todas las entidades
        public virtual DbSet<AbonoLiquidacion> AbonosLiquidacion { get; set; }
        public virtual DbSet<AsignacionesCartera> AsignacionesCarteras { get; set; }
        public virtual DbSet<BancosPagos> BancosPagos { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<CompromisosPago> CompromisosPagos { get; set; }
        public virtual DbSet<ConsultasExterna> ConsultasExternas { get; set; }
        public virtual DbSet<Deuda> Deudas { get; set; }
        public virtual DbSet<DeudorTelefono> DeudorTelefonos { get; set; }
        public virtual DbSet<Deudores> Deudores { get; set; }
        public virtual DbSet<FormaPago> FormasPago { get; set; }
        public virtual DbSet<Gestione> Gestiones { get; set; }
        public virtual DbSet<Pago> Pagos { get; set; }
        public virtual DbSet<RespuestaTipoContacto> RespuestasTipoContacto { get; set; }
        public virtual DbSet<TipoContactoGestion> TiposContactoGestion { get; set; }
        public virtual DbSet<TipoContactoResultado> TiposContactoResultado { get; set; }
        public virtual DbSet<TipoCuentaBancaria> TiposCuentaBancaria { get; set; }
        public virtual DbSet<TipoResultado> TiposResultado { get; set; }
        public virtual DbSet<TipoTransaccion> TiposTransaccion { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<TipoTarea> TiposTareas { get; set; }
        public virtual DbSet<MensajeWhatsappUsuario> MensajesWhatsapp { get; set; }
        public virtual DbSet<ImagenesCobros> ImagenesCobros { get; set; }
        public virtual DbSet<EmailSmtpConfig> EmailSmtpConfigs { get; set; }

        public DbSet<ArticuloOperacionCrecos> ArticuloOperacionCrecos => Set<ArticuloOperacionCrecos>();
        public DbSet<CarteraAsignadaCrecos> CarteraAsignadaCrecos => Set<CarteraAsignadaCrecos>();
        public DbSet<DatosClienteCrecos> DatosClienteCrecos => Set<DatosClienteCrecos>();
        public DbSet<DireccionClienteCrecos> DireccionClienteCrecos => Set<DireccionClienteCrecos>();
        public DbSet<OperacionesClientesCrecos> OperacionesClientesCrecos => Set<OperacionesClientesCrecos>();
        public DbSet<ReferenciasPersonalesCrecos> ReferenciasPersonalesCrecos => Set<ReferenciasPersonalesCrecos>();
        public DbSet<SaldoClienteCrecos> SaldoClienteCrecos => Set<SaldoClienteCrecos>();
        public DbSet<TelefonosClienteCrecos> TelefonosClienteCrecos => Set<TelefonosClienteCrecos>();

        public DbSet<ReciboDetalleCrecos> ReciboDetalleCrecos => Set<ReciboDetalleCrecos>();
        public DbSet<ReciboPagosCrecos> ReciboPagosCrecos => Set<ReciboPagosCrecos>();
        public DbSet<ReciboFormaPagoCrecos> ReciboFormaPagoCrecos => Set<ReciboFormaPagoCrecos>();

        public DbSet<TrifocusCrecos> TrifocusCrecos => Set<TrifocusCrecos>();
        public DbSet<CuotaOperacionCrecos> CuotasOperacionCrecos => Set<CuotaOperacionCrecos>();
        public DbSet<SeguimientoMensajes> SeguimientoMensajes => Set<SeguimientoMensajes>();
        public DbSet<MensajesEnviadosWhatsapp> MensajesEnviadosWhatsapp => Set<MensajesEnviadosWhatsapp>();
        public DbSet<TrifocusCrecosPartes> TrifocusCrecosPartes => Set<TrifocusCrecosPartes>();
        public DbSet<WhatsappSession> WhatsappSessions { get; set; } = default!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureSimpleEntities(modelBuilder);

            ConfigureComplexEntities(modelBuilder);

            ConfigureSpecialRelationships(modelBuilder);
        }

        private void ConfigureSimpleEntities(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TrifocusCrecosPartes>(e =>
            {
                e.ToTable("trifocuscrecospartes", "temp_crecos");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id)
                   .HasColumnType("varchar(36)");
                e.Property(e => e.CodOperacion)
                   .HasColumnName("codoperacion")
                   .HasColumnType("varchar");
                e.Property(e => e.ValorLiquidacion)
                   .HasColumnName("valorliquidacion")
                   .HasColumnType("numeric(18,2)");
                e.Property(e => e.ValorLiquidacionParte)
                   .HasColumnName("valorliquidacionparte")
                   .HasColumnType("numeric(18,2)");
            });

            modelBuilder.Entity<SeguimientoMensajes>(entity =>
            {
                entity.HasKey(x => x.id);
                entity.Property(e => e.id)
                    .HasColumnName("id")
                    .HasMaxLength(100);
                entity.Property(e => e.tipo)
                    .HasColumnType("varchar");
                entity.Property(e => e.numeroDestino)
                    .HasColumnType("varchar");
                entity.Property(e => e.usuario)
                    .HasColumnType("varchar");
                entity.Property(e => e.usuarioWhatsapp)
                    .HasColumnType("varchar");
                entity.Property(e => e.mensaje)
                    .HasColumnType("text");
                entity.Property(x => x.fechaRegistro).HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<MensajesEnviadosWhatsapp>(entity =>
            {
                entity.ToTable("MensajesEnviadosWhatsapp");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(100);

                entity.Property(e => e.IdCliente)
                    .HasColumnName("id_cliente")
                    .HasColumnType("varchar");

                entity.Property(e => e.IdUsuario)
                    .HasColumnName("id_usuario")
                    .HasColumnType("varchar");

                entity.Property(e => e.TelefonoEnviado)
                    .HasColumnName("telefono_enviado")
                    .HasColumnType("varchar");

                entity.Property(e => e.FechaRegistro)
                    .HasColumnName("fecha_registro")
                    .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<AuditLog>(e =>
            {
                e.ToTable("audit_logs", "audit");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.EventDateUtc).HasColumnType("timestamp with time zone");
                e.Property(x => x.KeyJson).HasColumnType("jsonb");
                e.Property(x => x.BeforeJson).HasColumnType("jsonb");
                e.Property(x => x.AfterJson).HasColumnType("jsonb");
                e.HasIndex(x => x.EventDateUtc);
                e.HasIndex(x => new { x.Table, x.Action });
            });


            modelBuilder.Entity<WhatsappSession>(entity =>
            {
                entity.HasIndex(x => x.User).IsUnique(false);
                entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
                entity.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");

                entity.HasOne(ws => ws.Usuario)
                  .WithMany(u => u.WhatsappSessions)
                  .HasForeignKey(ws => ws.IdUsuario)
                  .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ReciboPagosCrecos>(entity =>
            {
                entity.ToTable("ReciboPagosCrecos", schema: "temp_crecos");

                entity.HasKey(e => e.Id).HasName("PK_IdReciboPagosCrecos");

                entity.Property(e => e.Id)
                  .HasColumnName("Id")
                  .HasMaxLength(100);

                // Campos (TODOS explícitos)
                entity.Property(e => e.CodRecibo)
                    .HasColumnName("COD_RECIBO")
                    .HasMaxLength(100);

                entity.Property(e => e.CestadoRegistro)
                    .HasColumnName("CESTADO_REGISTRO")
                    .HasMaxLength(50);

                entity.Property(e => e.CodEmpresa)
                    .HasColumnName("COD_EMPRESA")
                    .HasMaxLength(50);

                entity.Property(e => e.DescripcEmpresa)
                    .HasColumnName("DESCRIPC_EMPRESA")
                    .HasMaxLength(200);

                entity.Property(e => e.NombreArchivo)
                    .HasColumnName("Nombre_Archivo")
                    .HasMaxLength(200);

                entity.Property(e => e.CodUNegocio)
                    .HasColumnName("COD_UNEGOCIO")
                    .HasMaxLength(50);

                entity.Property(e => e.DescripcUNegocio)
                    .HasColumnName("DESCRIPC_UNEGOCIO")
                    .HasMaxLength(200);

                entity.Property(e => e.CodTCartera)
                    .HasColumnName("COD_TCARTERA")
                    .HasMaxLength(50);

                entity.Property(e => e.DescripcTCartera)
                    .HasColumnName("DESCRIPC_TCARTERA")
                    .HasMaxLength(200);

                entity.Property(e => e.CodOficina)
                    .HasColumnName("COD_OFICINA")
                    .HasMaxLength(50);

                entity.Property(e => e.CDescripcionOficina)
                    .HasColumnName("CDESCRIPCION_OFICINA")
                    .HasMaxLength(200);

                entity.Property(e => e.NumIdentificacion)
                    .HasColumnName("NUM_IDENTIFICACION")
                    .HasMaxLength(50);

                entity.Property(e => e.CodPagoReferencial)
                    .HasColumnName("COD_PAGO_REFERENCIAL")
                    .HasMaxLength(100);

                entity.Property(e => e.CodMoneda)
                    .HasColumnName("COD_MONEDA")
                    .HasMaxLength(10);

                entity.Property(e => e.DescripcMoneda)
                    .HasColumnName("DESCRIPC_MONEDA")
                    .HasMaxLength(100);

                entity.Property(e => e.CodTPago)
                    .HasColumnName("COD_TPAGO")
                    .HasMaxLength(50);

                entity.Property(e => e.DescripcTPago)
                    .HasColumnName("DESCRIPC_TPAGO")
                    .HasMaxLength(200);

                entity.Property(e => e.CodCaja)
                    .HasColumnName("COD_CAJA")
                    .HasMaxLength(50);

                entity.Property(e => e.DescripcCaja)
                    .HasColumnName("DESCRIPC_CAJA")
                    .HasMaxLength(200);

                entity.Property(e => e.CodGestor)
                    .HasColumnName("COD_GESTOR")
                    .HasMaxLength(100);

                entity.Property(e => e.DescripcGestor)
                    .HasColumnName("DESCRIPC_GESTOR")
                    .HasMaxLength(200);

                entity.Property(e => e.CodTRecibo)
                    .HasColumnName("COD_TRECIBO")
                    .HasMaxLength(50);

                entity.Property(e => e.DescripcTRecibo)
                    .HasColumnName("DESCRIPC_TRECIBO")
                    .HasMaxLength(200);

                entity.Property(e => e.FechaPago)
                    .HasColumnName("FECHA_PAGO")
                    .HasColumnType("timestamp with time zone"); // Postgres

                entity.Property(e => e.DFechaReverso)
                    .HasColumnName("DFECHAREVERSO")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Monto)
                    .HasColumnName("MONTO")
                    .HasPrecision(18, 2);

                entity.Property(e => e.Cambio)
                    .HasColumnName("CAMBIO")
                    .HasPrecision(18, 6);
            });

            modelBuilder.Entity<ReciboDetalleCrecos>(entity =>
            {
                entity.ToTable("ReciboDetalleCrecos", schema: "temp_crecos");

                entity.HasKey(e => e.Id).HasName("PK_IdReciboDetalleCrecos");

                entity.HasIndex(e => e.CodRecibo).HasDatabaseName("IX_ReciboDetalle_CodRecibo");

                entity.Property(e => e.Id)
                   .HasColumnName("Id")
                   .HasMaxLength(200);

                entity.Property(e => e.NombreArchivo)
                    .HasColumnName("Nombre_Archivo")
                    .HasMaxLength(200);

                entity.Property(e => e.IReciboDetalle)
                    .HasColumnName("IRECIBODETALLE")
                    .HasMaxLength(100);

                entity.Property(e => e.CodRecibo)
                    .HasColumnName("COD_RECIBO")
                    .HasMaxLength(100);

                entity.Property(e => e.ICodigoOperacion)
                    .HasColumnName("ICODIGOOPERACION")
                    .HasMaxLength(100);

                entity.Property(e => e.NumCuota)
                    .HasColumnName("NUM_CUOTA");

                entity.Property(e => e.CodRubro)
                    .HasColumnName("COD_RUBRO")
                    .HasMaxLength(50);

                entity.Property(e => e.CDescripcionRubro)
                    .HasColumnName("CDESCRIPCION_RUBRO")
                    .HasMaxLength(200);

                entity.Property(e => e.ValorRecibo)
                    .HasColumnName("VALOR_RECIBO")
                    .HasPrecision(18, 2);
            });


            modelBuilder.Entity<ReciboFormaPagoCrecos>(entity =>
            {
                entity.ToTable("ReciboFormaPagoCrecos", schema: "temp_crecos");

                entity.HasKey(e => e.Id).HasName("PK_IdReciboFormaPagoCrecos");

                // Índice sugerido
                entity.HasIndex(e => e.CodRecibo).HasDatabaseName("IX_ReciboFormaPago_CodRecibo");

                entity.Property(e => e.Id)
                    .HasColumnName("Id")
                    .HasMaxLength(200);

                entity.Property(e => e.NombreArchivo)
                    .HasColumnName("Nombre_Archivo")
                    .HasMaxLength(200);

                entity.Property(e => e.CodReciboFormaPago)
                    .HasColumnName("COD_RECIBO_FORMAPAGO")
                    .HasMaxLength(100);

                entity.Property(e => e.CodRecibo)
                    .HasColumnName("COD_RECIBO")
                    .HasMaxLength(100);

                entity.Property(e => e.CodFormaPago)
                    .HasColumnName("COD_FORMA_PAGO")
                    .HasMaxLength(50);

                entity.Property(e => e.DescripcFPago)
                    .HasColumnName("DESCRIPC_FPAGO")
                    .HasMaxLength(200);

                entity.Property(e => e.CodInsFinanciera)
                    .HasColumnName("COD_INS_FINANCIERA")
                    .HasMaxLength(50);

                entity.Property(e => e.CDescripcionInstitucionFinanciera)
                    .HasColumnName("CDESCRIPCION_INSTITUCION_FINANCIERA")
                    .HasMaxLength(200);

                entity.Property(e => e.NumCuenta)
                    .HasColumnName("NUM_CUENTA")
                    .HasMaxLength(100);

                entity.Property(e => e.NumDocumento)
                    .HasColumnName("NUM_DOCUMENTO")
                    .HasMaxLength(100);

                entity.Property(e => e.CNombreCuentaCorrentista)
                    .HasColumnName("CNOMBRECUENTACORRENTISTA")
                    .HasMaxLength(200);

                entity.Property(e => e.CCedulaCuentaCorrentista)
                    .HasColumnName("CCEDULACUENTACORRENTISTA")
                    .HasMaxLength(50);

                entity.Property(e => e.DFechaCobroDocumento)
                    .HasColumnName("DFECHACOBRODOCUMENTO")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.CodMoneda)
                    .HasColumnName("COD_MONEDA")
                    .HasMaxLength(10);

                entity.Property(e => e.DescripcMoneda)
                    .HasColumnName("DESCRIPC_MONEDA")
                    .HasMaxLength(100);

                entity.Property(e => e.CodMotivo)
                    .HasColumnName("COD_MOTIVO")
                    .HasMaxLength(50);

                entity.Property(e => e.DescripcMotivo)
                    .HasColumnName("DESCRIPC_MOTIVO")
                    .HasMaxLength(200);

                entity.Property(e => e.IValor)
                    .HasColumnName("IVALOR")
                    .HasPrecision(18, 2);
            });

            modelBuilder.Entity<CuotaOperacionCrecos>(entity =>
            {
                entity.ToTable("CuotasOperacionCrecos", schema: "temp_crecos");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnName("id")
                      .HasColumnType("varchar");

                entity.Property(e => e.CodOperacion)
                      .HasColumnName("COD_OPERACION")
                      .HasMaxLength(60);

                entity.Property(e => e.CodCuota)
                      .HasColumnName("COD_CUOTA")
                      .HasMaxLength(60);

                entity.Property(e => e.NumeroCuota)
                      .HasColumnName("NUMERO_CUOTA");

                entity.Property(e => e.FechaVencimiento)
                      .HasColumnName("FECHA_VENCIMIENTO")
                      .HasColumnType("date");

                entity.Property(e => e.FechaCorte)
                      .HasColumnName("FECHA_CORTE")
                      .HasColumnType("date");

                entity.Property(e => e.FechaUltimoPago)
                      .HasColumnName("FECHA_ULTIMO_PAGO")
                      .HasColumnType("date");

                entity.Property(e => e.DFechaPostergacion)
                      .HasColumnName("DFECHAPOSTERGACION")
                      .HasColumnType("date");

                entity.Property(e => e.CodEstadoCuota)
                      .HasColumnName("COD_ESTADO_CUOTA")
                      .HasMaxLength(20);

                entity.Property(e => e.DescEstadoOperacion)
                      .HasColumnName("DESC_ESTADO_OPERACION")
                      .HasMaxLength(120);

                entity.Property(e => e.TasaMora)
                      .HasColumnName("TASA_MORA")
                      .HasColumnType("numeric(18,4)");

                entity.Property(e => e.CodEstadoRegistro)
                      .HasColumnName("COD_ESTADO_REGISTRO")
                      .HasMaxLength(20);

                entity.Property(e => e.DesEstadoRegistro)
                      .HasColumnName("DES_ESTADO_REGISTRO")
                      .HasMaxLength(120);

                entity.Property(e => e.IValorTotalCuota)
                      .HasColumnName("IVALORTOTALCUOTA")
                      .HasColumnType("numeric(18,2)");

                entity.Property(e => e.IValorCuota)
                      .HasColumnName("IVALORCUOTA")
                      .HasColumnType("numeric(18,2)");

                entity.Property(e => e.ValorCapitalInteres)
                      .HasColumnName("VALOR_CAPITAL_INTERES")
                      .HasColumnType("numeric(18,2)");

                entity.Property(e => e.ValorCargos)
                      .HasColumnName("VALOR_CARGOS")
                      .HasColumnType("numeric(18,2)");

                entity.Property(e => e.ValorOtrosCargos)
                      .HasColumnName("VALOR_OTROS_CARGOS")
                      .HasColumnType("numeric(18,2)");

                entity.HasIndex(e => e.CodOperacion)
                      .HasDatabaseName("IX_CuotasOperacion_CodOperacion");
            });



            modelBuilder.Entity<TrifocusCrecos>(e =>
                {
                    e.ToTable("TrifocusCrecos", schema: "temp_crecos");
                    e.HasKey(x => x.Id);

                    e.HasIndex(x => x.CodigoCliente);
                    e.HasIndex(x => x.IdentificacionCliente);
                    e.HasIndex(x => new
                    {
                        x.Operacion,
                        x.CodigoCliente
                    }).HasDatabaseName("ix_operacion_cliente");

                    // Strings "largos"
                    e.Property(x => x.DireccionDomicilio).HasMaxLength(500);
                    e.Property(x => x.DireccionTrabajo).HasMaxLength(500);
                    e.Property(x => x.DireccionNegocio).HasMaxLength(500);
                    e.Property(x => x.Referencia1).HasMaxLength(300);
                    e.Property(x => x.Referencia2).HasMaxLength(300);

                    // Strings "cortos"
                    e.Property(x => x.UsuarioGenera).HasMaxLength(100);
                    e.Property(x => x.GestorAsignado).HasMaxLength(120);
                    e.Property(x => x.Ciudad).HasMaxLength(120);
                    e.Property(x => x.CodigoCliente).HasMaxLength(60);
                    e.Property(x => x.IdentificacionCliente).HasMaxLength(20);
                    e.Property(x => x.NombreCliente).HasMaxLength(200);
                    e.Property(x => x.EstadoCivil).HasMaxLength(40);
                    e.Property(x => x.TipoCredito).HasMaxLength(80);
                    e.Property(x => x.TramoActual).HasMaxLength(60);
                    e.Property(x => x.Formalidad).HasMaxLength(40);
                    e.Property(x => x.Calificacion).HasMaxLength(40);
                    e.Property(x => x.Semaforo).HasMaxLength(20);
                    e.Property(x => x.Operacion).HasMaxLength(60);
                    e.Property(x => x.TRAMO).HasMaxLength(60);
                    e.Property(x => x.Cargo).HasMaxLength(120);
                    e.Property(x => x.NombreTrabajo).HasMaxLength(200);

                    // Teléfonos
                    e.Property(x => x.TelefonoDomicilio).HasMaxLength(30);
                    e.Property(x => x.CelularDomicilio).HasMaxLength(30);
                    e.Property(x => x.TelefonoTrabajo).HasMaxLength(30);
                    e.Property(x => x.CelularTrabajo).HasMaxLength(30);
                    e.Property(x => x.TelefonoNegocio).HasMaxLength(30);
                    e.Property(x => x.CelularNegocio).HasMaxLength(30);
                    e.Property(x => x.TelefonoReferencia1).HasMaxLength(30);
                    e.Property(x => x.TelefonoReferencia2).HasMaxLength(30);

                    // Decimales
                    e.Property(x => x.DeudaTotal).HasPrecision(18, 2);
                    e.Property(x => x.PagoMinimo).HasPrecision(18, 2);
                    e.Property(x => x.SaldoVencido).HasPrecision(18, 2);
                    e.Property(x => x.MontoUltimoPagado).HasPrecision(18, 2);
                    e.Property(x => x.MontoCarteraAsignada).HasPrecision(18, 2);
                    e.Property(x => x.MontoCobrado).HasPrecision(18, 2);
                    e.Property(x => x.ValLiquidacion).HasPrecision(18, 2);
                    e.Property(x => x.ValLiquidacionPartes).HasPrecision(18, 2);

                    // Fechas
                    e.Property(x => x.FechaCompromisodePago).HasColumnType("date");
                    e.Property(x => x.FechaUltimoPago).HasColumnType("date");
                    e.Property(x => x.UltimaGestionTerrena).HasColumnType("date");
                    e.Property(x => x.GestionTerrenaAnterior).HasColumnType("date");
                    e.Property(x => x.UltimaGestionTelefonica).HasColumnType("date");
                    e.Property(x => x.GestionTelefonicaAnterior).HasColumnType("date");

                    // Mapeo opcional de nombres exactos de columnas (si tu tabla ya existe con estos nombres)
                    e.Property(x => x.UsuarioGenera).HasColumnName("UsuarioGenera");
                    e.Property(x => x.GestorAsignado).HasColumnName("GestorAsignado");
                    e.Property(x => x.Ciudad).HasColumnName("Ciudad");
                    e.Property(x => x.CodigoCliente).HasColumnName("CodigoCliente");
                    e.Property(x => x.IdentificacionCliente).HasColumnName("IdentificacionCliente");
                    e.Property(x => x.NombreCliente).HasColumnName("NombreCliente");
                    e.Property(x => x.EstadoCivil).HasColumnName("EstadoCivil");
                    e.Property(x => x.TipoCredito).HasColumnName("TipoCredito");
                    e.Property(x => x.TramoActual).HasColumnName("TramoActual");
                    e.Property(x => x.Formalidad).HasColumnName("Formalidad");
                    e.Property(x => x.Calificacion).HasColumnName("Calificacion");
                    e.Property(x => x.NoCorte).HasColumnName("NoCorte");
                    e.Property(x => x.Semaforo).HasColumnName("Semaforo");
                    e.Property(x => x.Operacion).HasColumnName("Operacion");
                    e.Property(x => x.DeudaRefinanciada).HasColumnName("DeudaRefinanciada");
                    e.Property(x => x.FechaCompromisodePago).HasColumnName("FechaCompromisodePago");
                    e.Property(x => x.DiasVencidosIniciodeMes).HasColumnName("DiasVencidosIniciodeMes");
                    e.Property(x => x.TRAMO).HasColumnName("TRAMO");
                    e.Property(x => x.DiasVencidosActuales).HasColumnName("DiasVencidosActuales");
                    e.Property(x => x.DeudaTotal).HasColumnName("DeudaTotal");
                    e.Property(x => x.PagoMinimo).HasColumnName("PagoMinimo");
                    e.Property(x => x.SaldoVencido).HasColumnName("SaldoVencido");
                    e.Property(x => x.FechaUltimoPago).HasColumnName("FechaUltimoPago");
                    e.Property(x => x.MontoUltimoPagado).HasColumnName("MontoUltimoPagado");
                    e.Property(x => x.TelefonoDomicilio).HasColumnName("TelefonoDomicilio");
                    e.Property(x => x.CelularDomicilio).HasColumnName("CelularDomicilio");
                    e.Property(x => x.DireccionDomicilio).HasColumnName("DireccionDomicilio");
                    e.Property(x => x.TelefonoTrabajo).HasColumnName("TelefonoTrabajo");
                    e.Property(x => x.CelularTrabajo).HasColumnName("CelularTrabajo");
                    e.Property(x => x.Cargo).HasColumnName("Cargo");
                    e.Property(x => x.NombreTrabajo).HasColumnName("NombreTrabajo");
                    e.Property(x => x.DireccionTrabajo).HasColumnName("DireccionTrabajo");
                    e.Property(x => x.TelefonoNegocio).HasColumnName("TelefonoNegocio");
                    e.Property(x => x.CelularNegocio).HasColumnName("CelularNegocio");
                    e.Property(x => x.DireccionNegocio).HasColumnName("DireccionNegocio");
                    e.Property(x => x.Referencia1).HasColumnName("Referencia1");
                    e.Property(x => x.TelefonoReferencia1).HasColumnName("TelefonoReferencia1");
                    e.Property(x => x.Referencia2).HasColumnName("Referencia2");
                    e.Property(x => x.TelefonoReferencia2).HasColumnName("TelefonoReferencia2");
                    e.Property(x => x.MontoCarteraAsignada).HasColumnName("MontoCarteraAsignada");
                    e.Property(x => x.MontoCobrado).HasColumnName("MontoCobrado");
                    e.Property(x => x.PoseeVehiculo).HasColumnName("PoseeVehiculo");
                    e.Property(x => x.UltimaGestionTerrena).HasColumnName("ultimaGestionTerrena");
                    e.Property(x => x.GestionTerrenaAnterior).HasColumnName("gestionTerrenaAnterior");
                    e.Property(x => x.UltimaGestionTelefonica).HasColumnName("ultimaGestionTelefonica");
                    e.Property(x => x.GestionTelefonicaAnterior).HasColumnName("gestionTelefonicaAnterior");
                    e.Property(x => x.NoGestiones).HasColumnName("NoGestiones");
                    e.Property(x => x.NoCuotasPagadas).HasColumnName("NoCuotasPagadas");
                    e.Property(x => x.ValLiquidacion).HasColumnName("Valliquidacion");
                    e.Property(x => x.ValLiquidacionPartes).HasColumnName("Valliquidacionpartes");
                });




            modelBuilder.Entity<ArticuloOperacionCrecos>(e =>
            {
                e.ToTable("ArticuloOperacionCrecos", schema: "temp_crecos");
                e.HasKey(x => x.Id);
                e.Property(x => x.ISECUENCIAL);
                e.Property(x => x.COD_PRODUCTO).HasMaxLength(50);
                e.Property(x => x.COD_OPERACION).HasColumnType("varchar");
                e.Property(x => x.DESC_PRODUCTO).HasMaxLength(200);
                e.Property(x => x.CANTIDAD);
                e.Property(x => x.OBSERVACION).HasMaxLength(300);
                e.HasIndex(x => x.COD_OPERACION);
            });

            // ======================= CarteraAsignadaCrecos ========================
            modelBuilder.Entity<CarteraAsignadaCrecos>(e =>
            {
                e.ToTable("CarteraAsignadaCrecos", schema: "temp_crecos");
                e.HasKey(x => x.Id);

                e.Property(x => x.COD_EMPRESA).HasMaxLength(50);
                e.Property(x => x.EMPRESA).HasMaxLength(200);

                e.Property(x => x.COD_UNIDAD_NEGOCIO).HasMaxLength(50);
                e.Property(x => x.UNIDAD_NEGOCIO).HasMaxLength(200);

                e.Property(x => x.COD_TIPO_CARTERA).HasMaxLength(50);
                e.Property(x => x.TIPO_CARTERA).HasMaxLength(200);

                e.Property(x => x.IMES);
                e.Property(x => x.IANO);

                e.Property(x => x.CNUMEROIDENTIFICACION).HasMaxLength(50);
                e.Property(x => x.CNOMBRECOMPLETO).HasMaxLength(200);

                e.Property(x => x.COD_TIPO_GESTOR).HasMaxLength(50);
                e.Property(x => x.CDESCRIPCION).HasMaxLength(200);

                e.Property(x => x.BCUOTAIMPAGA).HasMaxLength(10);
                e.Property(x => x.DIAS_MORA);

                e.Property(x => x.DFECHAVENCIMIENTO);
                e.Property(x => x.IVALORDEUDATOTAL).HasColumnType("numeric(18,2)");

                e.Property(x => x.ICICLOCORTE);

                e.Property(x => x.COD_PAIS).HasMaxLength(50);
                e.Property(x => x.PAIS).HasMaxLength(150);

                e.Property(x => x.COD_PROVINCIA).HasMaxLength(50);
                e.Property(x => x.PROVINCIA).HasMaxLength(150);

                e.Property(x => x.COD_CANTON).HasMaxLength(50);
                e.Property(x => x.CANTON).HasMaxLength(150);

                e.Property(x => x.COD_ZONA).HasMaxLength(50);
                e.Property(x => x.ZONA).HasMaxLength(150);

                e.Property(x => x.COD_BARRIO).HasMaxLength(50);
                e.Property(x => x.BARRIO).HasMaxLength(150);

                e.Property(x => x.COD_GESTOR).HasMaxLength(50);
                e.Property(x => x.GESTOR).HasMaxLength(150);

                e.Property(x => x.CODIGOCLIENTE).HasMaxLength(50);

                e.HasIndex(x => x.CNUMEROIDENTIFICACION);
                e.HasIndex(x => new { x.IMES, x.IANO });
                e.HasIndex(x => x.CODIGOCLIENTE);
            });

            // ========================= DatosClienteCrecos ==========================
            modelBuilder.Entity<DatosClienteCrecos>(e =>
            {
                e.ToTable("DatosClienteCrecos", schema: "temp_crecos");
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.CNUMEROIDENTIFICACION);
                e.Property(x => x.CCODIGOTIPOPERSONA).HasMaxLength(50);
                e.Property(x => x.TIPO_PERSONA).HasMaxLength(100);
                e.Property(x => x.COD_TIPO_IDENTIF).HasMaxLength(50);
                e.Property(x => x.DESCRIP_TIPO_IDENTIF).HasMaxLength(100);
                e.Property(x => x.CNUMEROIDENTIFICACION).HasMaxLength(50);
                e.Property(x => x.CNOMBRECOMPLETO).HasMaxLength(200);
                e.Property(x => x.CEXENTOIMPUESTO).HasMaxLength(10);
                e.Property(x => x.CPRIMERNOMBRE).HasMaxLength(100);
                e.Property(x => x.CSEGUNDONOMBRE).HasMaxLength(100);
                e.Property(x => x.CAPELLIDOPATERNO).HasMaxLength(100);
                e.Property(x => x.CAPELLIDOMATERNO).HasMaxLength(100);
                e.Property(x => x.CCODIGOESTADOCIVIL).HasMaxLength(50);
                e.Property(x => x.DESCRIP_ESTADO_CIVIL).HasMaxLength(100);
                e.Property(x => x.CCODIGOSEXO).HasMaxLength(50);
                e.Property(x => x.DESCRIP_SEXO).HasMaxLength(100);
                e.Property(x => x.CCODIGOPAIS).HasMaxLength(50);
                e.Property(x => x.DESCRIP_PAIS).HasMaxLength(100);
                e.Property(x => x.CCODIGOCIUDADNACIMIENTO).HasMaxLength(50);
                e.Property(x => x.DESCRIP_CIUDAD_NACIMIENTO).HasMaxLength(150);
                e.Property(x => x.CCODIGONACIONALIDAD).HasMaxLength(50);
                e.Property(x => x.DESCRIP_NACIONALIDAD).HasMaxLength(100);
                e.Property(x => x.DFECHANACIMIENTO);
                e.Property(x => x.INUMEROCARGA);
                e.Property(x => x.CSEPARACIONBIEN).HasMaxLength(10);
                e.Property(x => x.CCODIGONIVELEDUCACION).HasMaxLength(50);
                e.Property(x => x.DESCRIP_NIVEL_EDUC).HasMaxLength(150);
                e.Property(x => x.CCODIGOTITULO).HasMaxLength(50);
                e.Property(x => x.DESCRIP_TITULO).HasMaxLength(150);
                e.Property(x => x.CRAZONCOMERCIAL).HasMaxLength(200);
                e.Property(x => x.CNOMBREEMPRESA).HasMaxLength(200);
                e.Property(x => x.CCODIGOCARGO).HasMaxLength(50);
                e.Property(x => x.DESCRIP_CARGO).HasMaxLength(150);
                e.Property(x => x.DFECHAINGRESOLAB);
                e.Property(x => x.IINGRESO).HasColumnType("numeric(18,2)");
                e.Property(x => x.IEGRESO).HasColumnType("numeric(18,2)");
                e.Property(x => x.ICODIGOCLIENTE).HasMaxLength(50);
                e.Property(x => x.ISCORE);
            });

            // ======================= DireccionClienteCrecos ========================
            modelBuilder.Entity<DireccionClienteCrecos>(e =>
            {
                e.ToTable("DireccionClienteCrecos", schema: "temp_crecos");
                e.HasKey(x => x.Id); // si hay varias por cliente, crea un Id
                e.Property(x => x.CNUMEROIDENTIFICACION).HasMaxLength(50);
                e.Property(x => x.CODIGO_UBICACION).HasMaxLength(50);
                e.Property(x => x.DESCRIP_UBICACION).HasMaxLength(150);
                e.Property(x => x.COD_PAIS).HasMaxLength(50);
                e.Property(x => x.DESCRIP_PAIS).HasMaxLength(150);
                e.Property(x => x.COD_PROVINCIA).HasMaxLength(50);
                e.Property(x => x.DESCRIP_PROVINCIA).HasMaxLength(150);
                e.Property(x => x.COD_CANTON).HasMaxLength(50);
                e.Property(x => x.DESCRIP_CANTON).HasMaxLength(150);
                e.Property(x => x.COD_PARROQUIA).HasMaxLength(50);
                e.Property(x => x.DESCRIP_PARROQUIA).HasMaxLength(150);
                e.Property(x => x.COD_BARRIO).HasMaxLength(50);
                e.Property(x => x.DESCRIP_BARRIO).HasMaxLength(150);
                e.Property(x => x.CDIRECCIONCARGA).HasMaxLength(250);
                e.Property(x => x.COBSERVACION).HasMaxLength(250);
                e.Property(x => x.CDIRECCIONCOMPLETA).HasMaxLength(300);
                e.Property(x => x.CCASILLA).HasMaxLength(50);
                e.Property(x => x.CCORREOELECTRONICO).HasMaxLength(150);
                e.Property(x => x.COD_TIPO_ESPACIO).HasMaxLength(50);
                e.Property(x => x.DESCRIP_TIPO_ESPACIO).HasMaxLength(150);
                e.Property(x => x.INUMEROESPACIO);
                e.Property(x => x.INUMEROSOLAR);
                e.Property(x => x.CCALLEPRINCIPAL).HasMaxLength(200);
                e.Property(x => x.CNUMEROCALLE).HasMaxLength(50);
                e.Property(x => x.CALLE_SECUND).HasMaxLength(200);
                e.Property(x => x.CALLE_SECUND_2).HasMaxLength(200);
                e.Property(x => x.CNUMERO_SOLAR).HasMaxLength(50);
                e.Property(x => x.COD_TIPO_NUMERACION).HasMaxLength(50);
                e.Property(x => x.DESCRIP_TIPO_NUMERACION).HasMaxLength(150);
                e.Property(x => x.COD_INDICADOR_POSICION).HasMaxLength(50);
                e.Property(x => x.DESCRIP_IND_POSICION).HasMaxLength(150);
                e.Property(x => x.NOMBRE_EDIFICIO).HasMaxLength(150);
                e.Property(x => x.CNUMEROPISO).HasMaxLength(20);
                e.Property(x => x.PISO_BLOQUE).HasMaxLength(50);
                e.Property(x => x.COFICINA_DEPARTAMENTO).HasMaxLength(100);
                e.Property(x => x.INDICADOR_PRINCIPAL).HasMaxLength(10);
                e.Property(x => x.COD_T_PROPIEDAD).HasMaxLength(50);
                e.Property(x => x.DESCRIP_TIPO_PROPIEDAD).HasMaxLength(150);
                e.Property(x => x.AÑO_ANTIGUEDAD);
                e.Property(x => x.MES_ANTIGUEDAD);
                e.Property(x => x.DIAS_ANTIGUEDAD);
            });

            // ======================= OperacionesClientesCrecos =====================
            modelBuilder.Entity<OperacionesClientesCrecos>(e =>
            {

                e.ToTable("OperacionesClientesCrecos", schema: "temp_crecos");
                e.HasKey(x => x.Id);
                e.Property(x => x.ICODIGOOPERACION).HasMaxLength(50);
                e.Property(x => x.COD_OFICINA).HasMaxLength(50);
                e.Property(x => x.DESC_OFICINA).HasMaxLength(150);
                e.Property(x => x.N_IDENTIFICACION).HasMaxLength(50);
                e.Property(x => x.NUM_FACTURA).HasMaxLength(50);
                e.Property(x => x.COD_MONEDA).HasMaxLength(10);
                e.Property(x => x.DESC_MONEDA).HasMaxLength(50);
                e.Property(x => x.COD_PROD_FINANCIERO).HasMaxLength(50);
                e.Property(x => x.DES_PROD_FINANCIERO).HasMaxLength(150);
                e.Property(x => x.ICODIGO_OPERACION_NEGOCIACION).HasMaxLength(50);
                e.Property(x => x.NUM_CUOTAS);
                e.Property(x => x.TASA_INTERES).HasPrecision(18, 2);
                e.Property(x => x.FECHA_FACTURA);
                e.Property(x => x.FECHA_ULTIMO_VENCIMIENTO);
                e.Property(x => x.FECHA_ULTMO_PAGO);
                e.Property(x => x.MONTO_CREDITO).HasPrecision(18, 2);
                e.Property(x => x.VALOR_FINANCIAR).HasPrecision(18, 2);
                e.Property(x => x.NUMERO_SOLICITUD).HasMaxLength(50);
                e.Property(x => x.COD_T_OPERACION).HasMaxLength(50);
                e.Property(x => x.DESC_T_OPERACION).HasMaxLength(150);
                e.Property(x => x.COD_T_CREDITO).HasMaxLength(50);
                e.Property(x => x.DESC_T_CREDITO).HasMaxLength(150);
                e.Property(x => x.COD_ESTADO_OPERACION).HasMaxLength(50);
                e.Property(x => x.DESC_ESTADO_OPERACION).HasMaxLength(150);
                e.Property(x => x.SECUENC_CUPO);
                e.Property(x => x.ESTADO_REGISTRO).HasMaxLength(20);
                e.Property(x => x.DES_ESTADO_REGISTRO).HasMaxLength(100);
                e.Property(x => x.COD_VENDEDOR).HasMaxLength(50);
                e.Property(x => x.DESC_VENDEDOR).HasMaxLength(150);
                e.HasIndex(x => x.N_IDENTIFICACION);
            });

            // ====================== ReferenciasPersonalesCrecos ====================
            modelBuilder.Entity<ReferenciasPersonalesCrecos>(e =>
            {
                e.ToTable("ReferenciasPersonalesCrecos", schema: "temp_crecos");
                e.HasKey(x => x.Id);
                e.Property(x => x.NumIdentificacion).HasMaxLength(50).HasColumnName("NUM_IDENTIFICACION");
                e.Property(x => x.NombreCompleto).HasMaxLength(200).HasColumnName("CNOMBRECOMPLETO");
                e.Property(x => x.CodTipoIdentRef).HasMaxLength(50).HasColumnName("COD_TIPO_IDENT_REF");
                e.Property(x => x.DescripTipoIdentific).HasMaxLength(150).HasColumnName("DESCRIPC_TIPO_IDENTIFIC");
                e.Property(x => x.NumIdentificRef).HasMaxLength(50).HasColumnName("NUM_IDENTIFIC_REF");
                e.Property(x => x.NombreReferencia).HasMaxLength(200).HasColumnName("NOMBRE_REFERENCIA");
                e.Property(x => x.CodPaisRef).HasMaxLength(50).HasColumnName("COD_PAIS_REF");
                e.Property(x => x.DescripPais).HasMaxLength(150).HasColumnName("DESCRIP_PAIS");
                e.Property(x => x.CodProvinciaRef).HasMaxLength(50).HasColumnName("COD_PROVINCIA_REF");
                e.Property(x => x.DescripProvincia).HasMaxLength(150).HasColumnName("DESCRIP_PROVINCIA");
                e.Property(x => x.CodCantonRef).HasMaxLength(50).HasColumnName("COD_CANTON_REF");
                e.Property(x => x.DescripCanton).HasMaxLength(150).HasColumnName("DESCRIP_CANTON");
                e.Property(x => x.CodTipoVinculoRef).HasMaxLength(50).HasColumnName("COD_TIPO_VINCULO_REF");
                e.Property(x => x.DescripVinculo).HasMaxLength(150).HasColumnName("DESCRIP_VINCULO");
                e.Property(x => x.DireccionRef).HasMaxLength(300).HasColumnName("DIRECCION_REF");
                e.Property(x => x.NumeroReferencia).HasMaxLength(50).HasColumnName("NUMERO_REFERENCIA");
            });

            // ========================= SaldoClienteCrecos ==========================
            modelBuilder.Entity<SaldoClienteCrecos>(e =>
            {
                e.ToTable("SaldoClienteCrecos", schema: "temp_crecos");
                // Clave compuesta sugerida (ajústala si el origen define otra):
                e.HasKey(x => x.Id);

                e.Property(x => x.COD_EMPRESA).HasMaxLength(50);
                e.Property(x => x.DESCP_EMPRESA).HasMaxLength(200);
                e.Property(x => x.COD_U_NEGOCIO).HasMaxLength(50);
                e.Property(x => x.DESC_U_NEGOCIO).HasMaxLength(200);
                e.Property(x => x.COD_CARTERA).HasMaxLength(50);
                e.Property(x => x.DESCRIP_CARTERA).HasMaxLength(200);
                e.Property(x => x.COD_GESTOR).HasMaxLength(50);
                e.Property(x => x.DESC_GESTOR).HasMaxLength(200);
                e.Property(x => x.IMES);
                e.Property(x => x.IANO);
                e.Property(x => x.COD_OFICINA).HasMaxLength(50);
                e.Property(x => x.CDESCRIPCION_OFICINA).HasMaxLength(200);
                e.Property(x => x.COD_TCREDITO).HasMaxLength(50);
                e.Property(x => x.DESCRIP_TCREDITO).HasMaxLength(200);
                e.Property(x => x.CNUMEROIDENTIFICACION).HasMaxLength(50);
                e.Property(x => x.COD_OPERACION).HasMaxLength(50);
                e.Property(x => x.CNUMEROTARJETA).HasMaxLength(50);
                e.Property(x => x.CICLO_CORTE).HasMaxLength(50);
                e.Property(x => x.DESC_CICLOCORTE).HasMaxLength(150);
                e.Property(x => x.DIAS_VENCIDOS);
                e.Property(x => x.ITRAMO);
                e.Property(x => x.CDESCRIPCIONTRAMO).HasMaxLength(200);
                e.Property(x => x.FECHA_MAX_PAGO);
                e.Property(x => x.VALOR_DEUDA).HasColumnType("numeric(18,2)");
                e.Property(x => x.VALOR_PAGO_MINIMO).HasColumnType("numeric(18,2)");
                e.Property(x => x.VALOR_CORRIENTE).HasColumnType("numeric(18,2)");
                e.Property(x => x.VALOR_VENCIDO).HasColumnType("numeric(18,2)");
                e.Property(x => x.VALOR_POR_VENCER).HasColumnType("numeric(18,2)");
                e.Property(x => x.VALOR_MORA).HasColumnType("numeric(18,2)");
                e.Property(x => x.VALOR_GESTION).HasColumnType("numeric(18,2)");
                e.Property(x => x.VALOR_VENCIDO_CORTEANTERIOR).HasColumnType("numeric(18,2)");
                e.Property(x => x.PRIMERA_CUOTA_VENCIDA);
                e.Property(x => x.NEGOCIACION_ACTIVA).HasMaxLength(10);
                e.Property(x => x.DFECHAEJECUCION);
                e.Property(x => x.FECHA_INGRESO);
                e.Property(x => x.CALIFICACION_CLIENTE).HasMaxLength(50);
                e.Property(x => x.F_ULTIMO_CORTE);
                e.Property(x => x.FECHA_ULT_PAGO);
                e.Property(x => x.VAL_ULT_PAGO).HasColumnType("numeric(18,2)");
                e.Property(x => x.VALOR_PAGO_MINIMO_ACTUALIZADO).HasColumnType("numeric(18,2)");
                e.Property(x => x.CODIGOCLIENTE).HasMaxLength(50);
                e.HasIndex(x => x.CNUMEROIDENTIFICACION);
            });

            // ======================== TelefonosClienteCrecos =======================
            modelBuilder.Entity<TelefonosClienteCrecos>(e =>
            {
                e.ToTable("TelefonosClienteCrecos", schema: "temp_crecos");
                e.HasKey(x => x.Id);
                e.Property(x => x.ISECUENCIA).HasColumnName("ISECUENCIA");
                e.Property(x => x.CNumeroIdentificacion).HasMaxLength(50).HasColumnName("CNUMEROIDENTIFICACION");
                e.Property(x => x.CodUbicacion).HasMaxLength(50).HasColumnName("COD_UBICACION");
                e.Property(x => x.DescripUbicacion).HasMaxLength(150).HasColumnName("DESCRIP_UBICACION");
                e.Property(x => x.CodTipoTelefono).HasMaxLength(50).HasColumnName("COD_TIPO_TELEFONO");
                e.Property(x => x.TipoTelefono).HasMaxLength(100).HasColumnName("TIPO_TELEFONO");
                e.Property(x => x.CNumero).HasMaxLength(50).HasColumnName("CNUMERO");
                e.Property(x => x.CPrefijo).HasMaxLength(10).HasColumnName("CPREFIJO");

                e.HasIndex(x => x.CNumeroIdentificacion);
            });


            modelBuilder.Entity<EmailSmtpConfig>(entity =>
            {
                entity.ToTable("EmailSmtpConfig"); // opcional: .ToTable("EmailSmtpConfig", "public")

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.SmtpHost)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.SmtpPort)
                    .IsRequired();
                entity.Property(e => e.UseSsl)
                    .IsRequired();
                entity.Property(e => e.UseStartTls)
                    .IsRequired();
                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
                entity.Property(e => e.TimeoutSeconds)
                    .HasDefaultValue(30);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt)
                    .IsRequired(false);
                entity.HasIndex(e => new { e.Key, e.IsActive })
                    .HasDatabaseName("IX_EmailConfig_Key_IsActive");
                entity.HasIndex(e => e.Key).IsUnique().HasFilter("\"IsActive\" = true").HasDatabaseName("UX_EmailConfig_Key_Active");

            });

            modelBuilder.Entity<ImagenesCobros>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
                entity.Property(e => e.NombreArchivo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Tamanio).HasMaxLength(50);
                entity.Property(e => e.UrlRelativo).HasMaxLength(500);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("NOW()"); ;
                entity.HasOne(ic => ic.Pago)
                    .WithMany(p => p.ImagenesCobrosNavigation)
                    .HasForeignKey(ic => ic.PagoId)
                    .HasPrincipalKey(p => p.IdPago)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MensajeWhatsappUsuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Mensaje).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.MensajeCorreo).HasColumnType("varchar");
                entity.Property(e => e.TipoMensaje).IsRequired().HasMaxLength(50);

            });

            modelBuilder.Entity<AbonoLiquidacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired();
                entity.Property(e => e.CodigoExterno).HasColumnType("varchar").IsRequired(false);

            });

            modelBuilder.Entity<TipoTarea>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre);
                entity.Property(e => e.Estado);
                entity.Property(e => e.CodigoExterno).HasColumnType("varchar");

            });

            modelBuilder.Entity<BancosPagos>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.Activo).IsRequired();
            });

            modelBuilder.Entity<FormaPago>(entity =>
            {
                entity.HasKey(e => e.FormaPagoId);
                entity.Property(e => e.FormaPagoId).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.Estado).IsRequired();
            });

            modelBuilder.Entity<TipoCuentaBancaria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired();
            });

            modelBuilder.Entity<TipoTransaccion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired();
            });

            modelBuilder.Entity<TipoResultado>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.CodigoEmpresaExterna).HasMaxLength(255);
                entity.Property(e => e.Activo).IsRequired();
            });
        }

        private void ConfigureComplexEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoContactoGestion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.CodigoEmpresaExterna).HasMaxLength(255);
                entity.Property(e => e.Estado).IsRequired();
            });

            modelBuilder.Entity<RespuestaTipoContacto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.Activo).IsRequired();
                entity.Property(e => e.CodigoEmpresaExterna).HasMaxLength(255);
                entity.Property(e => e.IdTipoContactoResultado).IsRequired();
            });

            modelBuilder.Entity<TipoContactoResultado>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.CodigoEmpresaExterna).HasMaxLength(255);
                entity.Property(e => e.Activo).IsRequired();
                entity.Property(e => e.TipoResultadoId).IsRequired().HasMaxLength(50);

                entity.HasOne(e => e.TipoResultadoNavigation)
                        .WithMany()
                        .HasForeignKey(e => e.TipoResultadoId)
                        .OnDelete(DeleteBehavior.Restrict);


                entity.HasMany(e => e.TiposRespuestaNavigation)
                    .WithOne(e => e.TipoContactoNavigatorNavigation)
                    .HasForeignKey(e => e.IdTipoContactoResultado)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureSpecialRelationships(ModelBuilder modelBuilder)
        {
            // Configuración de relaciones para Pago
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasKey(e => e.IdPago);
                entity.Property(e => e.IdPago).HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.MontoPagado).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NumeroDocumenro).HasMaxLength(50);
                entity.Property(e => e.Observaciones).HasMaxLength(500);
                entity.Property(e => e.ArchivoMigracion).HasMaxLength(500);
                entity.Property(e => e.FechaPago).HasDefaultValueSql("CURRENT_DATE");
                entity.Property(e => e.FechaRegistro).HasColumnType("timestamptz")
                                      .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Telefono)
                      .HasColumnType("varchar")
                      .HasDefaultValue("")
                      .IsRequired(false);
                entity.HasOne(p => p.FormaPagoNavigation)
                    .WithMany()
                    .HasForeignKey(p => p.FormaPagoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.BancosNavigation)
                    .WithMany()
                    .HasForeignKey(p => p.IdBancosPago)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.TipoCuentaBancariaNavigation)
                    .WithMany()
                    .HasForeignKey(p => p.IdTipoCuentaBancaria)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.TipoTransaccionNavigation)
                    .WithMany()
                    .HasForeignKey(p => p.IdTipoTransaccion)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.AbonoLiquidacionNavigation)
                    .WithMany()
                    .HasForeignKey(p => p.IdAbonoLiquidacion)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.IdUsuarioNavigation)
                    .WithMany()
                    .HasForeignKey(p => p.IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.IdDeudaNavigation)
                    .WithMany(d => d.Pagos)
                    .HasForeignKey(p => p.IdDeuda)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de relaciones para Gestione
            modelBuilder.Entity<Gestione>(entity =>
            {
                entity.HasKey(e => e.IdGestion);
                entity.Property(e => e.IdGestion).HasMaxLength(40);
                entity.Property(e => e.Descripcion).HasMaxLength(900);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FechaGestion).HasColumnType("timestamptz")
                                                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Telefono)
                     .HasColumnType("varchar")
                     .HasDefaultValue("")
                     .IsRequired(false);
                // Relaciones actualizadas
                entity.HasOne(g => g.IdUsuarioGestionaNavigation)
                    .WithMany(u => u.Gestiones)
                    .HasForeignKey(g => g.IdUsuarioGestiona)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(g => g.IdTipoContactoResultadoNavigation)
                    .WithMany()
                    .HasForeignKey(g => g.IdTipoContactoResultado)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(g => g.RespuestaTipoContactoNavigation)
                    .WithMany()
                    .HasForeignKey(g => g.IdRespuestaTipoContacto)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(g => g.IdDeudaNavigation)
                    .WithMany(d => d.Gestiones)
                    .HasForeignKey(g => g.IdDeuda)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración para AsignacionesCartera
            modelBuilder.Entity<AsignacionesCartera>(entity =>
            {
                entity.HasKey(e => e.IdAsignacion);
                entity.Property(e => e.Estado).HasMaxLength(50).HasDefaultValue("activo");
                entity.Property(e => e.IdCliente).HasMaxLength(13).IsRequired();
                entity.Property(e => e.IdDeuda).IsRequired();
                entity.Property(e => e.FechaAsignacion).IsRequired();

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.AsignacionesCarteras)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.IdDeudaNavigation)
                    .WithMany(p => p.AsignacionesCarteras)
                    .HasForeignKey(d => d.IdDeuda)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // Configuración para Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente);
                entity.Property(e => e.IdCliente).HasMaxLength(13);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Direccion).HasMaxLength(255);
                entity.Property(e => e.TelefonoContacto).HasMaxLength(50);
                entity.Property(e => e.Correo).HasMaxLength(100);

                entity.HasIndex(e => e.Cedula);
            });

            // Configuración para CompromisosPago
            modelBuilder.Entity<CompromisosPago>(entity =>
            {
                entity.HasKey(e => e.IdCompromiso);
                entity.Property(e => e.IdCompromiso).HasMaxLength(40);
                entity.Property(e => e.Estado);
                entity.Property(e => e.FechaCompromiso).IsRequired();
                entity.Property(e => e.IncumplioCompromisoPago);
                entity.Property(e => e.HoraRecordatorio);
                entity.Property(e => e.MontoComprometido).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.IdUsuario).HasMaxLength(13).IsRequired();
                entity.Property(e => e.IdTipoTarea).HasMaxLength(40);
                entity.Property(e => e.FechaRegistro).HasColumnType("timestamptz")
                                                     .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Telefono)
                     .HasColumnType("varchar")
                     .HasDefaultValue("")
                     .IsRequired(false);
                entity.HasOne(d => d.IdDeudaNavigation)
                    .WithMany(p => p.CompromisosPagos)
                    .HasForeignKey(d => d.IdDeuda)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.CompromisosPagos)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.IdTipoTareaNavigation)
                    .WithMany(p => p.CompromisosPagos)
                    .HasForeignKey(d => d.IdTipoTarea)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            // Configuración para ConsultasExterna
            modelBuilder.Entity<ConsultasExterna>(entity =>
            {
                entity.HasKey(e => e.IdDeudor);
                entity.Property(e => e.IdDeudor).HasMaxLength(13);
                entity.Property(e => e.JsonRespuesta).IsRequired();
                entity.Property(e => e.FechaConsulta).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.IdDeudorNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdDeudor)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Deuda>(entity =>
            {
                entity.HasKey(e => e.IdDeuda);
                entity.Property(e => e.DeudaCapital).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Interes).HasColumnType("decimal(18,2)");
                entity.Property(e => e.GastosCobranzas).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SaldoDeuda).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Descuento);
                entity.Property(e => e.CodigoOperacion).HasColumnType("varchar");
                entity.Property(e => e.MontoCobrar).HasColumnType("decimal(18,2)");
                entity.Property(e => e.MontoCobrarPartes).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FechaVenta);
                entity.Property(e => e.FechaUltimoPago);

                entity.Property(e => e.FechaRegistro)
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("now()")
                      .ValueGeneratedOnAdd();
                entity.Property(e => e.CodigoEmpresa);

                entity.Property(e => e.Estado);
                entity.Property(e => e.ProductoDescripcion)
                      .HasColumnType("varchar")
                      .HasDefaultValue("")
                      .IsRequired(false);
                entity.Property(e => e.Agencia)
                      .HasColumnType("varchar")
                      .HasDefaultValue("")
                      .IsRequired(false);
                entity.Property(e => e.Ciudad)
                      .HasColumnType("varchar")
                      .HasDefaultValue("")
                      .IsRequired(false);
                entity.Property(e => e.EsActivo)
                      .HasColumnType("bool")
                      .HasDefaultValue(true)
                      .IsRequired(false);
                entity.Property(e => e.DiasMora);
                entity.Property(e => e.NumeroFactura);
                entity.Property(e => e.Clasificacion);
                entity.Property(e => e.Creditos);
                entity.Property(e => e.NumeroCuotas);
                entity.Property(e => e.TipoDocumento);
                entity.Property(e => e.ValorCuota).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Tramo);
                entity.Property(e => e.UltimoPago).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Empresa);
                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(13)
                    .IsRequired(false);
                entity.HasIndex(e => e.IdUsuario);
                entity.HasIndex(e => e.NumeroFactura);
                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Deudas)
                    .HasForeignKey(e => e.IdUsuario)
                    .OnDelete(DeleteBehavior.SetNull);


                entity.HasOne(d => d.IdDeudorNavigation)
                    .WithMany(p => p.Deuda)
                    .HasForeignKey(d => d.IdDeudor)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<DeudorTelefono>(entity =>
            {
                entity.HasKey(e => e.IdDeudorTelefonos);
                entity.Property(e => e.IdDeudorTelefonos).HasMaxLength(40);
                entity.Property(e => e.IdDeudor).HasMaxLength(13);
                entity.Property(e => e.Telefono).HasMaxLength(12);
                entity.Property(e => e.FechaAdicion).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Origen).HasMaxLength(100);
                entity.Property(e => e.Observacion).HasMaxLength(500);
                entity.Property(e => e.Propietario).HasColumnType("varchar").HasMaxLength(500);

                entity.HasOne(d => d.IdDeudorNavigation)
                    .WithMany(p => p.DeudorTelefonos)
                    .HasForeignKey(d => d.IdDeudor)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración para Deudores
            modelBuilder.Entity<Deudores>(entity =>
            {
                entity.HasKey(e => e.IdDeudor);
                entity.Property(e => e.IdDeudor).HasMaxLength(36);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Telefono).HasMaxLength(50);
                entity.Property(e => e.Direccion).HasColumnType("varchar");
                entity.Property(e => e.Empresa).HasMaxLength(255);
                entity.Property(e => e.Correo).HasMaxLength(100);
                entity.Property(e => e.CodigoDeudor).HasMaxLength(100);
                entity.Property(e => e.FechaRegistro)
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("now()")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.Descripcion).HasColumnType("varchar");
                entity.Property(e => e.IdUsuario).HasMaxLength(13);

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.Deudores)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.SetNull);
            });


            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);
                entity.HasIndex(e => e.IdUsuario);
                entity.Property(e => e.IdUsuario).HasMaxLength(13);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Telefono).HasMaxLength(50);
                entity.Property(e => e.EstaActivo)
                      .HasColumnType("bool")
                      .HasDefaultValue(true);
                entity.Property(e => e.CodigoUsuario).HasMaxLength(50);
                entity.Property(e => e.NombreUsuario).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Rol)
                        .HasMaxLength(20)
                        .HasColumnName("rol");
                entity.Property(e => e.Contrasena).IsRequired().HasMaxLength(255);
            });

            // Configuración de relaciones para TipoContactoResultado
            modelBuilder.Entity<TipoContactoResultado>(entity =>
            {
                entity.HasOne(e => e.TipoResultadoNavigation)
                    .WithMany(t => t.TiposConstactosNavigation)
                    .HasForeignKey(e => e.TipoResultadoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de relaciones para TipoContactoGestion
            modelBuilder.Entity<TipoContactoGestion>(entity =>
            {
                entity.HasMany(e => e.RespuestaTipoContactos)
                    .WithOne()
                    .HasForeignKey(r => r.Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}