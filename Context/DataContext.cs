using gestiones_backend.Entity;
using Microsoft.EntityFrameworkCore;

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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para entidades simples
            ConfigureSimpleEntities(modelBuilder);

            ConfigureComplexEntities(modelBuilder);

            ConfigureSpecialRelationships(modelBuilder);
        }

        private void ConfigureSimpleEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AbonoLiquidacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Activo).IsRequired();
            });

            modelBuilder.Entity<TipoTarea>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre);
                entity.Property(e => e.Estado);

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
                entity.Property(e => e.Estado).IsRequired();
            });

            modelBuilder.Entity<RespuestaTipoContacto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
                entity.Property(e => e.Activo).IsRequired();
                entity.Property(e => e.IdTipoContactoResultado).IsRequired();
            });

            modelBuilder.Entity<TipoContactoResultado>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
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
                entity.Property(e => e.FechaPago).HasDefaultValueSql("CURRENT_DATE");
                entity.Property(e => e.FechaRegistro).HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
                
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
                entity.Property(e => e.FechaGestion).HasDefaultValueSql("CURRENT_TIMESTAMP");

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
                entity.Property(e => e.FechaRegistro).HasColumnType("timestamp without time zone").HasDefaultValueSql("NOW()");
                
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

            // Configuración para Deuda
            modelBuilder.Entity<Deuda>(entity =>
            {
                entity.HasKey(e => e.IdDeuda);
                entity.Property(e => e.DeudaCapital).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Interes).HasColumnType("decimal(18,2)");
                entity.Property(e => e.GastosCobranzas).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SaldoDeuda).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Descuento);
                entity.Property(e => e.MontoCobrar).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FechaVenta);
                entity.Property(e => e.FechaUltimoPago);
                entity.Property(e => e.Estado);
                entity.Property(e => e.DiasMora);
                entity.Property(e => e.NumeroFactura);
                entity.Property(e => e.Clasificacion);
                entity.Property(e => e.Creditos);
                entity.Property(e => e.SaldoDeulda).HasColumnType("decimal(18,2)");
                entity.Property(e => e.NumeroCuotas);
                entity.Property(e => e.TipoDocumento);
                entity.Property(e => e.ValorCuota).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Tramo);
                entity.Property(e => e.UltimoPago).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Empresa);

                entity.HasOne(d => d.IdDeudorNavigation)
                    .WithMany(p => p.Deuda)
                    .HasForeignKey(d => d.IdDeudor)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración para DeudorTelefono
            modelBuilder.Entity<DeudorTelefono>(entity =>
            {
                entity.HasKey(e => e.IdDeudorTelefonos);
                entity.Property(e => e.IdDeudorTelefonos).HasMaxLength(40);
                entity.Property(e => e.IdDeudor).HasMaxLength(13);
                entity.Property(e => e.Telefono).HasMaxLength(12);
                entity.Property(e => e.FechaAdicion).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Origen).HasMaxLength(100);
                entity.Property(e => e.Observacion).HasMaxLength(500);

                entity.HasOne(d => d.IdDeudorNavigation)
                    .WithMany(p => p.DeudorTelefonos)
                    .HasForeignKey(d => d.IdDeudor)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración para Deudores
            modelBuilder.Entity<Deudores>(entity =>
            {
                entity.HasKey(e => e.IdDeudor);
                entity.Property(e => e.IdDeudor).HasMaxLength(13);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Telefono).HasMaxLength(50);
                entity.Property(e => e.Direccion).HasMaxLength(255);
                entity.Property(e => e.Correo).HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.IdUsuario).HasMaxLength(13);

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.Deudores)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.SetNull);
            });

          
            // Configuración para Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.IdUsuario).HasMaxLength(13);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Telefono).HasMaxLength(50);
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