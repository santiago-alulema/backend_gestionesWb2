using gestiones_backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Context
{
    public partial  class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public virtual DbSet<AsignacionesCartera> AsignacionesCarteras { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<CompromisosPago> CompromisosPagos { get; set; }
        public virtual DbSet<ConsultasExterna> ConsultasExternas { get; set; }
        public virtual DbSet<Deuda> Deudas { get; set; }
        public virtual DbSet<DeudorTelefono> DeudorTelefonos { get; set; }
        public virtual DbSet<Deudores> Deudores { get; set; }
        public virtual DbSet<Gestione> Gestiones { get; set; }
        public virtual DbSet<Pago> Pagos { get; set; }
        public virtual DbSet<TiposGestion> TiposGestions { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AsignacionesCartera>(entity =>
            {
                entity.HasKey(e => e.IdAsignacion).HasName("asignaciones_cartera_pkey");

                entity.ToTable("asignaciones_cartera");
                
                entity.Property(e => e.IdAsignacion).HasColumnName("id_asignacion");
                entity.Property(e => e.Estado)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'activo'::character varying")
                    .HasColumnName("estado");
                entity.Property(e => e.FechaAsignacion).HasColumnName("fecha_asignacion");
                entity.Property(e => e.FechaRetiro).HasColumnName("fecha_retiro");
                entity.Property(e => e.IdCliente)
                    .HasMaxLength(13)
                    .HasColumnName("id_cliente");
                entity.Property(e => e.IdDeuda)
                    .HasMaxLength(36)
                    .HasColumnName("id_deuda");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");

                entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.AsignacionesCarteras)
                    .HasForeignKey(d => d.IdCliente)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("asignaciones_cartera_clientes_fk");

                entity.HasOne(d => d.IdDeudaNavigation).WithMany(p => p.AsignacionesCarteras)
                    .HasForeignKey(d => d.IdDeuda)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("asignaciones_cartera_deudas_fk");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente).HasName("clientes_pkey");

                entity.ToTable("clientes");

                entity.Property(e => e.IdCliente)
                    .HasMaxLength(13)
                    .HasColumnName("id_cliente");
                entity.Property(e => e.Correo)
                    .HasMaxLength(100)
                    .HasColumnName("correo");
                entity.Property(e => e.Direccion)
                    .HasMaxLength(255)
                    .HasColumnName("direccion");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(255)
                    .HasColumnName("nombre");
                entity.Property(e => e.TelefonoContacto)
                    .HasMaxLength(50)
                    .HasColumnName("telefono_contacto");
            });

            modelBuilder.Entity<CompromisosPago>(entity =>
            {
                entity.HasKey(e => e.IdCompromiso).HasName("compromisos_pago_pkey");

                entity.ToTable("compromisos_pago");

                entity.Property(e => e.IdCompromiso)
                   .HasColumnType("varchar(40)")
                   .HasColumnName("id_compromiso")
                   .HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.Estado)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'pendiente'::character varying")
                    .HasColumnName("estado");
                entity.Property(e => e.FechaCompromiso).HasColumnName("fecha_compromiso");
                entity.Property(e => e.FechaCumplimientoReal).HasColumnName("fecha_cumplimiento_real");
                entity.Property(e => e.IdDeuda)
                    .HasMaxLength(36)
                    .HasColumnName("id_deuda");
                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(13)
                    .HasColumnName("id_usuario");
                entity.Property(e => e.MontoComprometido)
                    .HasPrecision(12, 2)
                    .HasColumnName("monto_comprometido");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");

                entity.HasOne(d => d.IdDeudaNavigation).WithMany(p => p.CompromisosPagos)
                    .HasForeignKey(d => d.IdDeuda)
                    .HasConstraintName("compromisos_pago_deudas_fk");

                entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.CompromisosPagos)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("compromisos_pago_usuarios_fk");
            });

            modelBuilder.Entity<ConsultasExterna>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("consultas_externas");

                entity.Property(e => e.FechaConsulta)
                    .HasDefaultValueSql("now()")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_consulta");
                entity.Property(e => e.IdDeudor)
                    .HasMaxLength(13)
                    .HasColumnName("id_deudor");
                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(13)
                    .HasColumnName("id_usuario");
                entity.Property(e => e.JsonRespuesta)
                    .HasColumnType("json")
                    .HasColumnName("json_respuesta");

                entity.HasOne(d => d.IdDeudorNavigation).WithMany()
                    .HasForeignKey(d => d.IdDeudor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("consutlas_externas_deudores_fk");

                entity.HasOne(d => d.IdUsuarioNavigation).WithMany()
                    .HasForeignKey(d => d.IdUsuario)
                    .HasConstraintName("consutlas_externas_usuarios_fk");
            });

            modelBuilder.Entity<Deuda>(entity =>
            {
                entity.HasKey(e => e.IdDeuda).HasName("deudas_pkey");

                entity.ToTable("deudas");

                entity.Property(e => e.IdDeuda)
                    .HasMaxLength(36)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasColumnName("id_deuda");

                entity.Property(e => e.IdDeudor)
                    .HasMaxLength(13)
                    .HasColumnName("id_deudor");

                entity.Property(e => e.MontoOriginal)
                    .HasPrecision(12, 2)
                    .HasColumnName("monto_original");

                entity.Property(e => e.SaldoActual)
                    .HasPrecision(12, 2)
                    .HasColumnName("saldo_actual");

                entity.Property(e => e.FechaVencimiento)
                    .HasColumnName("fecha_vencimiento");

                entity.Property(e => e.FechaAsignacion)
                    .HasColumnName("fecha_asignacion");

                entity.Property(e => e.Estado)
                    .HasMaxLength(50)
                    .HasColumnName("estado");

                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion");

                entity.Property(e => e.NumeroFactura)
                    .HasColumnName("numero_factura");

                entity.Property(e => e.TotalFactura)
                    .HasPrecision(12, 2)
                    .HasColumnName("total_factura");

                entity.Property(e => e.NumeroAutorizacion)
                    .HasColumnName("numero_autorizacion");

                entity.Property(e => e.SaldoDeuda)
                    .HasPrecision(12, 2)
                    .HasColumnName("saldo_deuda");

                entity.Property(e => e.NumeroCuotas)
                    .HasColumnName("numero_cuotas");

                entity.Property(e => e.CuotaActual)
                    .HasColumnName("cuota_actual");

                entity.Property(e => e.ValorCuota)
                    .HasPrecision(12, 2)
                    .HasColumnName("valor_cuota");

                entity.Property(e => e.Tramo)
                    .HasColumnName("tramo");

                entity.Property(e => e.UltimoPago)
                    .HasPrecision(12, 2)
                    .HasColumnName("ultimo_pago");

                entity.Property(e => e.Empresa)
                    .HasColumnName("empresa");

                entity.HasOne(d => d.IdDeudorNavigation)
                    .WithMany(p => p.Deuda)
                    .HasForeignKey(d => d.IdDeudor)
                    .HasConstraintName("deudas_deudores_fk");
            });


            modelBuilder.Entity<DeudorTelefono>(entity =>
            {
                entity.HasKey(e => e.IdDeudorTelefonos).HasName("deudor_telefonos_pk");

                entity.ToTable("deudor_telefonos");

                entity.HasIndex(e => new { e.IdDeudor, e.Telefono }, "deudor_telefonos_unique").IsUnique();

                entity.Property(e => e.IdDeudorTelefonos)
                    .HasColumnType("varchar(40)")
                    .HasColumnName("id_deudor_telefonos")
                    .HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.EsValido).HasColumnName("es_valido");
                entity.Property(e => e.FechaAdicion)
                    .HasDefaultValueSql("now()")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_adicion");
                entity.Property(e => e.IdDeudor)
                    .HasMaxLength(13)
                    .HasColumnName("id_deudor");
                entity.Property(e => e.Origen)
                    .HasColumnType("text")
                    .HasColumnName("origen");
                entity.Property(e => e.Observacion)
                   .HasColumnType("text")
                   .HasColumnName("observaciones");
                entity.Property(e => e.Telefono)
                    .HasMaxLength(12)
                    .HasColumnName("telefono");

                entity.HasOne(d => d.IdDeudorNavigation).WithMany(p => p.DeudorTelefonos)
                    .HasForeignKey(d => d.IdDeudor)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("deudor_telefonos_deudores_fk");
            });

            modelBuilder.Entity<Deudores>(entity =>
            {
                entity.HasKey(e => e.IdDeudor).HasName("deudores_pkey");

                entity.ToTable("deudores");

                entity.Property(e => e.IdDeudor)
                    .HasMaxLength(13)
                    .HasColumnName("id_deudor");
                entity.Property(e => e.Correo)
                    .HasMaxLength(100)
                    .HasColumnName("correo");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.Direccion)
                    .HasMaxLength(255)
                    .HasColumnName("direccion");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(255)
                    .HasColumnName("nombre");
                entity.Property(e => e.Telefono)
                    .HasMaxLength(50)
                    .HasColumnName("telefono");
                entity.HasOne(d => d.Usuario)
        .WithMany(p => p.Deudores)
        .HasForeignKey(d => d.IdUsuario)
        .HasConstraintName("fk_deudores_usuario")
        .OnDelete(DeleteBehavior.SetNull);


            });

            modelBuilder.Entity<Gestione>(entity =>
            {
                entity.HasKey(e => e.IdGestion).HasName("gestiones_pkey");

                entity.ToTable("gestiones");

                entity.Property(e => e.IdGestion)
                   .HasColumnType("varchar(40)")
                   .HasColumnName("id_gestion")
                   .HasDefaultValueSql("gen_random_uuid()");
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.FechaGestion)
                    .HasDefaultValueSql("now()")
                    .HasColumnType("timestamp without time zone")
                    .HasColumnName("fecha_gestion");
                entity.Property(e => e.IdDeuda)
                    .HasMaxLength(36)
                    .HasColumnName("id_deuda");
                entity.Property(e => e.IdTipoGestion).HasMaxLength(36).HasColumnName("id_tipo_gestion");
                entity.Property(e => e.IdUsuarioGestiona)
                    .HasMaxLength(13)
                    .HasColumnName("id_usuario_gestiona");

                entity.HasOne(d => d.IdDeudaNavigation).WithMany(p => p.Gestiones)
                    .HasForeignKey(d => d.IdDeuda)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gestiones_deudas_fk");

                entity.HasOne(d => d.IdTipoGestionNavigation).WithMany(p => p.Gestiones)
                    .HasForeignKey(d => d.IdTipoGestion)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gestiones_id_tipo_gestion_fkey");

                entity.HasOne(d => d.IdUsuarioGestionaNavigation).WithMany(p => p.Gestiones)
                    .HasForeignKey(d => d.IdUsuarioGestiona)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("gestiones_usuarios_fk");
            });

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasKey(e => e.IdPago).HasName("pagos_pkey");

                entity.ToTable("pagos");

                entity.Property(e => e.IdPago).HasColumnName("id_pago");
                entity.Property(e => e.FechaPago).HasColumnName("fecha_pago");
                entity.Property(e => e.IdDeuda)
                    .HasMaxLength(36)
                    .HasColumnName("id_deuda");
                entity.Property(e => e.MedioPago)
                    .HasMaxLength(50)
                    .HasColumnName("medio_pago");
                entity.Property(e => e.MontoPagado)
                    .HasPrecision(12, 2)
                    .HasColumnName("monto_pagado");
                entity.Property(e => e.Observaciones).HasColumnName("observaciones");

                entity.HasOne(d => d.IdDeudaNavigation).WithMany(p => p.Pagos)
                    .HasForeignKey(d => d.IdDeuda)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pagos_deudas_fk");
            });

            modelBuilder.Entity<TiposGestion>(entity =>
            {
                entity.HasKey(e => e.IdTipoGestion).HasName("tipos_gestion_pkey");

                entity.ToTable("tipos_gestion");

                entity.Property(e => e.IdTipoGestion)
                   .HasColumnType("varchar(40)")
                   .HasColumnName("id_tipo_gestion")
                   .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.Descripcion).HasColumnName("descripcion");
                entity.Property(e => e.IdPadre).HasColumnName("id_padre");

                entity.Property(e => e.Estado)
                .HasColumnName("estado")
                .HasColumnType("boolean"); 
                entity.Property(e => e.Nombre)
                    .HasMaxLength(255)
                    .HasColumnName("nombre");
                entity.Property(e => e.TipoGestion)
                    .HasColumnType("varchar(20)")
                    .HasComment("P Padre H Hijo")
                    .HasColumnName("tipo_gestion");

                entity.HasOne(d => d.IdPadreNavigation).WithMany(p => p.InverseIdPadreNavigation)
                    .HasForeignKey(d => d.IdPadre)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("tipos_gestion_id_padre_fkey");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario).HasName("usuarios_pkey");

                entity.ToTable("usuarios");

                entity.Property(e => e.IdUsuario)
                    .HasMaxLength(13)
                    .HasColumnName("id_usuario");
                entity.Property(e => e.Contrasena)
                    .HasMaxLength(255)
                    .HasColumnName("contrasena");
                entity.Property(e => e.NombreUsuario)
                    .HasMaxLength(255)
                    .HasColumnName("nombre");
                entity.Property(e => e.Rol)
                    .HasMaxLength(1)
                    .HasDefaultValueSql("'U'::character varying")
                    .HasComment("U usuario A Admin")
                    .HasColumnName("rol");

              



            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}