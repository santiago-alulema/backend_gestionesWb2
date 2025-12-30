using CsvHelper;
using CsvHelper.Configuration;
using gestiones_backend.Context;
using gestiones_backend.DbConn;
using gestiones_backend.Entity;
using gestiones_backend.Entity.temp_crecos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql.Bulk;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;

namespace gestiones_backend.Services
{
    public class DeudoresImportService
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        private readonly string _root;

        public DeudoresImportService(DataContext db, 
                                     IWebHostEnvironment env,
                                     IConfiguration configuration   )
        {
            _dataContext = db;
            _root = Path.Combine(env.ContentRootPath, "ArchivosExternos");
            _configuration = configuration;
        }


        static DateTime? ToUtc(DateTime? dt)
                    => dt?.ToUniversalTime();

        public string importarPagos() {

            string cadena = $@"
                               INSERT INTO public.""Pagos"" (
                                      ""IdDeuda"",
                                      ""FechaPago"",
                                      ""MontoPagado"",
                                      ""MedioPago"",
                                      ""NumeroDocumenro"",
                                      ""Observaciones"",
                                      ""FormaPagoId"",
                                      ""IdUsuario"",
                                      ""IdBancosPago"",
                                      ""IdTipoCuentaBancaria"",
                                      ""IdTipoTransaccion"",
                                      ""IdAbonoLiquidacion"",
                                      ""UsuarioIdUsuario"",
                                      ""FechaRegistro"",
                                      ""IdPago"",
                                      ""Telefono"",
                                      ""ArchivoMigracion""
                                    )
                                    SELECT DISTINCT ON (rdc.""COD_RECIBO"")
	                                    d.""IdDeuda"" 					as ""IdDeuda"",
	                                    Date(rpc.""FECHA_PAGO"")		as ""FechaPago"",
	                                    totals.total_valor_recibo	    as ""MontoPagado"",
	                                    NULL::text                      as ""MedioPago"",
	                                    rpc.""COD_RECIBO""				as ""NumeroDocumenro"",

                                         (
                                           '[MIGRACION CRECOS] ' ||
                                           'TipoPago: '   || COALESCE(rfpc.""DESCRIPC_FPAGO"", '') ||
                                           ' Desc. Pago: '|| COALESCE(rpc.""DESCRIPC_TPAGO"", '') ||
                                           ' Motivo: '    || COALESCE(rfpc.""DESCRIPC_MOTIVO"", '')
                                         )::text                       as ""Observaciones"",
	                                    NULL::uuid AS ""FormaPagoId"",
	                                    (
                                            SELECT d2.""IdUsuario""
                                            FROM ""Deudas"" d2
                                            WHERE rpc.""NUM_IDENTIFICACION"" = d2.""IdDeudor""
                                            ORDER BY d2.""IdDeuda"" DESC
                                            LIMIT 1
                                        ) 								as ""IdUsuario"",
                                         NULL::uuid AS ""IdBancosPago"",
                                         NULL::uuid AS ""IdTipoCuentaBancaria"",
                                         NULL::uuid AS ""IdTipoTransaccion"",
                                         NULL::uuid AS ""IdAbonoLiquidacion"",
                                         NULL::uuid AS ""UsuarioIdUsuario"",
                                         rpc.""FECHA_PAGO""        AS ""FechaRegistro"",
                                         gen_random_uuid() AS ""IdPago"",
                                         ''::varchar  AS ""Telefono"",
	                                    rpc.""Nombre_Archivo""
	
                                        --rpc.""NUM_IDENTIFICACION"",
                                        --rpc.""COD_RECIBO"",
                                       -- rdc.""IRECIBODETALLE"",
                                       -- totals.total_valor_recibo,
                                       -- rpc.""DESCRIPC_TPAGO"",
                                       -- rdc.""ICODIGOOPERACION"",
                                        --rfpc.""DESCRIPC_FPAGO"",
                                       -- rfpc.""IVALOR"",
                                       -- rfpc.""DESCRIPC_MOTIVO"",
                                       -- rdc.""COD_RECIBO"" AS ""NumeroFactura"",
                                        --d.""CodigoOperacion"" 
	                                    FROM temp_crecos.""ReciboPagosCrecos"" rpc
	                                    JOIN temp_crecos.""ReciboDetalleCrecos"" rdc
	                                        ON rpc.""COD_RECIBO"" = rdc.""COD_RECIBO""
	                                    LEFT JOIN temp_crecos.""ReciboFormaPagoCrecos"" rfpc
	                                        ON rfpc.""COD_RECIBO"" = rpc.""COD_RECIBO""
	                                    LEFT JOIN ""Deudas"" d
	                                        ON   d.""IdDeudor""  = rpc.""NUM_IDENTIFICACION""   -- d.""CodigoOperacion"" = rdc.""ICODIGOOPERACION""
	                                    JOIN (
	                                        SELECT ""COD_RECIBO"", SUM(""VALOR_RECIBO"") AS total_valor_recibo
	                                        FROM temp_crecos.""ReciboDetalleCrecos""
                                            where  ""CDESCRIPCION_RUBRO"" <> 'Iva S.Gest.Adm.Pag.' and 
         		                                   ""CDESCRIPCION_RUBRO"" <> 'S.Gest.Adm. Pag.'
	                                        GROUP BY ""COD_RECIBO""
	                                    ) totals
	                                        ON totals.""COD_RECIBO"" = rdc.""COD_RECIBO""
	                                    WHERE rpc.""DESCRIPC_TPAGO"" <> 'Nota de Cr�dito' 
	                                    and  NOT EXISTS (
                                       SELECT 1
                                       FROM public.""Pagos"" p
                                       WHERE 
                                        Date(p.""FechaPago"")       = Date(rpc.""FECHA_PAGO"") AND 
                                        totals.total_valor_recibo   = p.""MontoPagado""
                                        AND p.""NumeroDocumenro"" = rpc.""COD_RECIBO""
                                     )
	                                    ORDER BY rdc.""COD_RECIBO"", rdc.""IRECIBODETALLE"";";

            PgConn conn = new PgConn();
            conn.cadenaConnect = _configuration.GetConnectionString("DefaultConnection");
            conn.ejecutarconsulta_dt(cadena);
            return "Se actualizo correctamente";
        }

        public string RedimencionarDeudasNoGestionadas() {
            string cadena = @$"SELECT *
                                FROM ""Deudas"" d
                                WHERE d.""Empresa"" LIKE '%CRECO%'
                                AND NOT EXISTS (
                                    SELECT 1
                                    FROM ""Pagos"" p
                                    WHERE p.""IdDeuda"" = d.""IdDeuda""
                                )
                                AND NOT EXISTS (
                                    SELECT 1
                                    FROM ""Gestiones"" g
                                    WHERE g.""IdDeuda"" = d.""IdDeuda""
                                )
                                AND NOT EXISTS (
                                    SELECT 1
                                    FROM ""CompromisosPagos"" cp
                                    WHERE cp.""IdDeuda"" = d.""IdDeuda""
                                );";
            List<Deuda> rows = _dataContext.Deudas
                                       .FromSqlRaw(cadena)
                                       .AsNoTracking()
                                       .ToList();

            List<Deudores> deudores = _dataContext.Deudores.ToList();
            var idsValidos = deudores
                .Where(d => d.IdDeudor != null)
                .Select(d => d.IdDeudor!)
                .ToHashSet();

            List<Usuario> usuarios = _dataContext.Usuarios
                .Where(x => x.Rol == "user" && x.EstaActivo)
                .ToList();

            rows = rows
                .Where(r => r.IdDeudor != null && idsValidos.Contains(r.IdDeudor!))
                .ToList();

            List<Deuda> rowsMenos180 = rows.Where(x => x.DiasMora < 180).ToList();
            List<Deuda> rows181a360 = rows.Where(x => x.DiasMora >= 180 && x.DiasMora <= 360).ToList();
            List<Deuda> rowsMas360 = rows.Where(x => x.DiasMora > 360).ToList();

            void Asignar(List<Deuda> lista)
            {
                if (usuarios.Count == 0) return;

                // Solo deudas sin asignar
                var pendientes = lista;
                if (pendientes.Count == 0) return;

                var grupos = pendientes
                    .GroupBy(d => d.IdDeudor)
                    .Select(g => new { Items = g.ToList(), Total = g.Sum(x => x.DeudaCapital ?? 0m) })
                    .OrderByDescending(g => g.Total)
                    .ThenByDescending(g => g.Items.Count)
                    .ToList();

                // Cargas solo para el reparto actual (no cuentan las ya asignadas)
                var cargas = usuarios.ToDictionary(u => u.IdUsuario, _ => 0m);

                foreach (var g in grupos)
                {
                    var target = cargas.OrderBy(kv => kv.Value).First().Key;

                    foreach (var d in g.Items) // todos están sin usuario
                        d.IdUsuario = target;

                    cargas[target] += g.Total;
                }
            }

            Asignar(rowsMenos180);
            Asignar(rows181a360);
            Asignar(rowsMas360);

            var todas = rowsMenos180.Concat(rows181a360).Concat(rowsMas360).ToList();

            List<Deuda> deudas = _dataContext.Deudas.ToList();


            foreach (var deuda in todas)
            {
                var existente = deudas.FirstOrDefault(d => d.NumeroFactura == deuda.NumeroFactura);

                if (existente != null)
                {

                    existente.DeudaCapital = deuda.DeudaCapital;
                    existente.Interes = deuda.Interes;
                    existente.GastosCobranzas = deuda.GastosCobranzas;
                    existente.SaldoDeuda = deuda.SaldoDeuda;
                    existente.Descuento = deuda.Descuento;
                    existente.MontoCobrar = deuda.MontoCobrar;
                    existente.FechaVenta = deuda.FechaVenta;
                    existente.FechaUltimoPago = deuda.FechaUltimoPago;
                    existente.Estado = deuda.Estado;
                    existente.DiasMora = deuda.DiasMora;
                    existente.NumeroFactura = deuda.NumeroFactura;
                    existente.Clasificacion = deuda.Clasificacion;
                    existente.Creditos = deuda.Creditos;
                    existente.ValorCuota = deuda.ValorCuota;
                    existente.EsActivo = true;
                    existente.Empresa = deuda.Empresa;
                    existente.Tramo = deuda.Tramo;
                    existente.UltimoPago = deuda.UltimoPago;

                    existente.MontoCobrarPartes = deuda.MontoCobrarPartes;
                    existente.FechaRegistro = ToUtc(existente.FechaRegistro);

                    if (string.IsNullOrEmpty(existente.IdUsuario))
                        existente.IdUsuario = deuda.IdUsuario;

                    _dataContext.Deudas.Update(existente);

                }
                else
                {
                    if (deuda.IdDeuda == Guid.Empty)
                        deuda.IdDeuda = Guid.NewGuid();
                    deuda.FechaRegistro = ToUtc(deuda.FechaRegistro);
                    deuda.EsActivo = true;
                    _dataContext.Deudas.Add(deuda);
                }
            }
            _dataContext.SaveChanges();

            return ("Se insertó y actualizó correctamente");
        }

        public string ImportarDeudas()
        {
            _dataContext.Database.ExecuteSqlRaw(@"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";");

            const string SqlDeudasCrecos = @"
                            SELECT 
                                gen_random_uuid()                                                              AS ""IdDeuda"",
                                MAX(occ.""DESC_T_CREDITO"")                                                    AS ""Estado"",
                                ca.""CNUMEROIDENTIFICACION""                                                   AS ""IdDeudor"",
                                string_agg(DISTINCT ('Fact:' || occ.""NUM_FACTURA""), ' - ')                   AS ""NumeroFactura"",
                                CAST(MAX(occ.""FECHA_FACTURA"") AS date)                                       AS ""FechaVenta"",
                                ROUND(MAX(scc.""VALOR_DEUDA""), 2)::numeric(18,2)                              AS ""SaldoDeuda"",
                                COALESCE(MAX(occ.""NUM_CUOTAS""), 0)::int                                      AS ""NumeroCuotas"",
                                COALESCE(MAX(scc.""DIAS_VENCIDOS""), 0)::int                                   AS ""DiasMora"",
                                0::numeric(18,2)                                                               AS ""ValorCuota"",
                                MAX(scc.""CDESCRIPCIONTRAMO"")::text                                           AS ""Tramo"",
                                NULL::numeric(18,2)                                                            AS ""UltimoPago"",
                                'CRECOSCORP'                                                                   AS ""Empresa"",
                                NULL::text                                                                     AS ""Clasificacion"",
                                1::int                                                                         AS ""Creditos"",
                                ROUND(
                                    (
                                    ROUND(MAX(scc.""VALOR_DEUDA""), 2)
                                    - COALESCE(ROUND(MAX(t.valorliquidacion), 2), 0)
                                    )
                                    / NULLIF(ROUND(MAX(scc.""VALOR_DEUDA""), 2), 0) * 100 , 0)                 AS ""Descuento"",
                                ROUND(MAX(scc.""VALOR_DEUDA""), 2)::numeric(18,2)                              AS ""DeudaCapital"",
                                CAST(MAX(scc.""FECHA_ULT_PAGO"") AS date)                                      AS ""FechaUltimoPago"",
                                ROUND(MAX(scc.""VALOR_GESTION""), 2)::numeric(18,2)                            AS ""GastosCobranzas"",
                                ROUND(MAX(scc.""VALOR_MORA""), 2)::numeric(18,2)                               AS ""Interes"",
                                ROUND(MAX(COALESCE(t.valorpontealdia,0)), 2)::numeric(18,2)                                AS ""MontoPonteAlDia"",
                                ROUND(
                                    COALESCE(MAX(t.valorliquidacion), 0),
                                    2
                                )::numeric(18,2)                                                               AS ""MontoCobrar"",
                                ROUND(MAX(t.valorliquidacionparte), 2)::numeric(18,2)                          AS ""MontoCobrarPartes"",
                                MAX(dcc.""DESCRIP_TIPO_IDENTIF"")::text                                        AS ""TipoDocumento"",
                                'Creditos Economicos'                                                          AS ""Agencia"",
                                MAX(ca.""CANTON"")::text                                                       AS ""Ciudad"",
                                (
                                    SELECT string_agg(
                                        '<strong>' || occ2.""NUM_FACTURA"" || '</strong>: ' || aoc.""DESC_PRODUCTO"", 
                                        ' || '
                                    )
                                    FROM temp_crecos.""ArticuloOperacionCrecos"" aoc
                                    INNER JOIN temp_crecos.""OperacionesClientesCrecos"" occ2
                                        ON occ2.""ICODIGOOPERACION"" = aoc.""COD_OPERACION""
                                    WHERE occ2.""N_IDENTIFICACION"" = ca.""CNUMEROIDENTIFICACION""
                                )                                                                               AS ""ProductoDescripcion"",
                                TRUE                                                                            AS ""EsActivo"",
                                NULL::varchar                                                                   AS ""IdUsuario"",
                                MAX(ca.""COD_EMPRESA"")::varchar                                                AS ""CodigoEmpresa"",
                                NOW()::timestamp                                                                AS ""FechaRegistro"",
                                MAX(scc.""COD_OPERACION"")::varchar                                           AS ""CodigoOperacion""
                            FROM temp_crecos.""CarteraAsignadaCrecos"" ca
                            LEFT JOIN temp_crecos.""DatosClienteCrecos"" dcc 
                                ON dcc.""ICODIGOCLIENTE"" = ca.""CODIGOCLIENTE""
                            LEFT JOIN temp_crecos.""OperacionesClientesCrecos"" occ 
                                ON occ.""N_IDENTIFICACION"" = ca.""CNUMEROIDENTIFICACION""
                            LEFT JOIN temp_crecos.trifocuscrecospartes t 
                                ON t.codoperacion = ca.""CNUMEROIDENTIFICACION""
                            LEFT JOIN temp_crecos.""SaldoClienteCrecos"" scc 
                                ON ca.""CODIGOCLIENTE"" = scc.""CODIGOCLIENTE""
                            GROUP BY ca.""CNUMEROIDENTIFICACION"";";



            List<Deuda> CarteraAsignada = _dataContext.Deudas
                                        .FromSqlRaw(SqlDeudasCrecos)
                                        .AsNoTracking()
                                        .ToList();

            _dataContext.Deudas
                             .Where(d => d.Empresa == "CRECOSCORP")
                             .ExecuteUpdate(setters => setters
                             .SetProperty(d => d.EsActivo, false));

            List<Usuario> usuarios = _dataContext.Usuarios
                .Where(x => x.EstaActivo && x.Rol == "user")
                .ToList();

            
            List<Deuda> deudas = _dataContext.Deudas.Where(x => x.Empresa.Contains("CRECO")).ToList();
            List<Deuda> deudasUpdate = new List<Deuda>();
            List<Deuda> deudasNuevo = new List<Deuda>();

            int contador = 0;
            foreach (var deuda in CarteraAsignada)
            {
                var existente = deudas.FirstOrDefault(d => d.IdDeudor == deuda.IdDeudor.ToString());

                if (existente != null)
                {

                    existente.DeudaCapital = deuda.DeudaCapital;
                    existente.Interes = deuda.Interes;
                    existente.GastosCobranzas = deuda.GastosCobranzas;
                    existente.SaldoDeuda = deuda.SaldoDeuda;
                    existente.Descuento = deuda.Descuento;
                    existente.MontoCobrar = deuda.MontoCobrar;
                    existente.FechaVenta = deuda.FechaVenta;
                    existente.FechaUltimoPago = deuda.FechaUltimoPago;
                    existente.Estado = deuda.Estado;
                    existente.DiasMora = deuda.DiasMora;
                    existente.NumeroFactura = deuda.NumeroFactura;
                    existente.Clasificacion = deuda.Clasificacion;
                    existente.Creditos = deuda.Creditos;
                    existente.ValorCuota = deuda.ValorCuota;
                    existente.EsActivo = true;
                    existente.Empresa = deuda.Empresa;
                    existente.MontoPonteAlDia = deuda.MontoPonteAlDia;
                    existente.Tramo = deuda.Tramo;
                    existente.UltimoPago = deuda.UltimoPago;
                    existente.CodigoOperacion = deuda.CodigoOperacion;
                    existente.MontoCobrarPartes = deuda.MontoCobrarPartes;
                    existente.FechaRegistro = DateTime.UtcNow;

                    if (string.IsNullOrEmpty(existente.IdUsuario))
                        existente.IdUsuario = usuarios[contador].IdUsuario;

                    deudasUpdate.Add(existente);
                }
                else
                {
                    if (deuda.IdDeuda == Guid.Empty)
                        deuda.IdDeuda = Guid.NewGuid();
                    deuda.FechaRegistro = DateTime.UtcNow;
                    deuda.CodigoOperacion = deuda.CodigoOperacion;
                    deuda.EsActivo = true;
                    deuda.IdUsuario = usuarios[contador].IdUsuario;
                    deudasNuevo.Add(deuda);
                }
                contador++;
                if (contador == usuarios.Count) contador = 0;
            }

            var bulk = new NpgsqlBulkUploader(_dataContext);
            bulk.Update(deudasUpdate);
            bulk.Insert(deudasNuevo);

            return ("Se insertó y actualizó correctamente");
        }


        public string ImportarDeudasSinCampania()
        {
            _dataContext.Database.ExecuteSqlRaw(@"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";");

            const string SqlDeudasCrecos = @"
                            SELECT 
                                gen_random_uuid()                                                              AS ""IdDeuda"",
                                MAX(occ.""DESC_T_CREDITO"")                                                    AS ""Estado"",
                                ca.""CNUMEROIDENTIFICACION""                                                   AS ""IdDeudor"",
                                string_agg(DISTINCT ('Fact:' || occ.""NUM_FACTURA""), ' - ')                   AS ""NumeroFactura"",
                                CAST(MAX(occ.""FECHA_FACTURA"") AS date)                                       AS ""FechaVenta"",
                                ROUND(MAX(scc.""VALOR_DEUDA""), 2)::numeric(18,2)                              AS ""SaldoDeuda"",
                                COALESCE(MAX(occ.""NUM_CUOTAS""), 0)::int                                      AS ""NumeroCuotas"",
                                COALESCE(MAX(scc.""DIAS_VENCIDOS""), 0)::int                                   AS ""DiasMora"",
                                0::numeric(18,2)                                                               AS ""ValorCuota"",
                                MAX(scc.""CDESCRIPCIONTRAMO"")::text                                           AS ""Tramo"",
                                NULL::numeric(18,2)                                                            AS ""UltimoPago"",
                                'CRECOSCORP'                                                                   AS ""Empresa"",
                                NULL::text                                                                     AS ""Clasificacion"",
                                1::int                                                                         AS ""Creditos"",
                                ROUND(
    (
        1 - (
            COALESCE(
                ROUND(MAX(ca.""IVALORDEUDATOTAL""), 2),
                ROUND(MAX(scc.""VALOR_DEUDA""), 2)  -- si no hay valor de liquidación, descuento = 0
            )
            /
            NULLIF(ROUND(MAX(scc.""VALOR_DEUDA""), 2), 0)
        )
    ) * 100
, 0) AS ""Descuento"",
                                ROUND(MAX(scc.""VALOR_DEUDA""), 2)::numeric(18,2)                              AS ""DeudaCapital"",
                                CAST(MAX(scc.""FECHA_ULT_PAGO"") AS date)                                      AS ""FechaUltimoPago"",
                                ROUND(MAX(scc.""VALOR_GESTION""), 2)::numeric(18,2)                            AS ""GastosCobranzas"",
                                ROUND(MAX(scc.""VALOR_MORA""), 2)::numeric(18,2)                               AS ""Interes"",
                                ROUND(MAX(COALESCE(0,0)), 2)::numeric(18,2)                    AS ""MontoPonteAlDia"",
                                ROUND(MAX(ca.""IVALORDEUDATOTAL""), 2)::numeric(18,2)                        AS ""MontoCobrar"",
                                ROUND(MAX(0), 2)::numeric(18,2)                          AS ""MontoCobrarPartes"",
                                MAX(dcc.""DESCRIP_TIPO_IDENTIF"")::text                                        AS ""TipoDocumento"",
                                'Creditos Economicos'                                                          AS ""Agencia"",
                                MAX(ca.""CANTON"")::text                                                       AS ""Ciudad"",
                                (
                                    SELECT string_agg(
                                        '<strong>' || occ2.""NUM_FACTURA"" || '</strong>: ' || aoc.""DESC_PRODUCTO"", 
                                        ' || '
                                    )
                                    FROM temp_crecos.""ArticuloOperacionCrecos"" aoc
                                    INNER JOIN temp_crecos.""OperacionesClientesCrecos"" occ2
                                        ON occ2.""ICODIGOOPERACION"" = aoc.""COD_OPERACION""
                                    WHERE occ2.""N_IDENTIFICACION"" = ca.""CNUMEROIDENTIFICACION""
                                )                                                                               AS ""ProductoDescripcion"",
                                TRUE                                                                            AS ""EsActivo"",
                                NULL::varchar                                                                   AS ""IdUsuario"",
                                MAX(ca.""COD_EMPRESA"")::varchar                                                AS ""CodigoEmpresa"",
                                NOW()::timestamp                                                                AS ""FechaRegistro"",
                                MAX(occ.""ICODIGOOPERACION"")::varchar                                          AS ""CodigoOperacion""
                            FROM temp_crecos.""CarteraAsignadaCrecos"" ca
                            LEFT JOIN temp_crecos.""DatosClienteCrecos"" dcc 
                                ON dcc.""ICODIGOCLIENTE"" = ca.""CODIGOCLIENTE""
                            LEFT JOIN temp_crecos.""OperacionesClientesCrecos"" occ 
                                ON occ.""N_IDENTIFICACION"" = ca.""CNUMEROIDENTIFICACION""
                            LEFT JOIN temp_crecos.trifocuscrecospartes t 
                                ON t.codoperacion = ca.""CNUMEROIDENTIFICACION""
                            LEFT JOIN temp_crecos.""SaldoClienteCrecos"" scc 
                                ON ca.""CODIGOCLIENTE"" = scc.""CODIGOCLIENTE""
                            GROUP BY ca.""CNUMEROIDENTIFICACION"";";



            List<Deuda> rows = _dataContext.Deudas
                                        .FromSqlRaw(SqlDeudasCrecos)
                                        .AsNoTracking()
                                        .ToList();

            _dataContext.Deudas
                             .Where(d => d.Empresa == "CRECOSCORP")
                             .ExecuteUpdate(setters => setters
                             .SetProperty(d => d.EsActivo, false));

            List<Deudores> deudores = _dataContext.Deudores.ToList();
            var idsValidos = deudores
                .Where(d => d.IdDeudor != null)
                .Select(d => d.IdDeudor!)
                .ToHashSet();

            List<Usuario> usuarios = _dataContext.Usuarios
                .Where(x => x.Rol == "user" && x.EstaActivo)
                .ToList();

            rows = rows
                .Where(r => r.IdDeudor != null && idsValidos.Contains(r.IdDeudor!))
                .ToList();

            List<Deuda> rowsMenos180 = rows.Where(x => x.DiasMora < 180).ToList();
            List<Deuda> rows181a360 = rows.Where(x => x.DiasMora >= 180 && x.DiasMora <= 360).ToList();
            List<Deuda> rowsMas360 = rows.Where(x => x.DiasMora > 360).ToList();

            void AsignarUsuario(List<Deuda> lista)
            {
                if (usuarios.Count == 0) return;

                var pendientes = lista.Where(d => d.IdUsuario == null).ToList();
                if (pendientes.Count == 0) return;

                int i = 0;
                foreach (var d in pendientes)
                {
                    d.IdUsuario = usuarios[i].IdUsuario;
                    i = (i + 1) % usuarios.Count;
                }
            }

            void Asignar(List<Deuda> lista)
            {
                if (usuarios.Count == 0) return;

                var pendientes = lista.Where(d => d.IdUsuario == null).ToList();
                if (pendientes.Count == 0) return;

                var grupos = pendientes
                    .GroupBy(d => d.IdDeudor)
                    .Select(g => new { Items = g.ToList(), Total = g.Sum(x => x.DeudaCapital ?? 0m) })
                    .OrderByDescending(g => g.Total)
                    .ThenByDescending(g => g.Items.Count)
                    .ToList();

                var cargas = usuarios.ToDictionary(u => u.IdUsuario, _ => 0m);

                foreach (var g in grupos)
                {
                    var target = cargas.OrderBy(kv => kv.Value).First().Key;

                    foreach (var d in g.Items) // todos están sin usuario
                        d.IdUsuario = target;

                    cargas[target] += g.Total;
                }
            }

            Asignar(rowsMenos180);
            Asignar(rows181a360);
            Asignar(rowsMas360);

            var todas = rowsMenos180.Concat(rows181a360).Concat(rowsMas360).ToList();

            List<Deuda> deudas = _dataContext.Deudas.ToList();

            List<Deuda> deudasUpdate = new List<Deuda>();
            List<Deuda> deudasNuevo = new List<Deuda>();

            foreach (var deuda in todas)
            {
                var existente = deudas.FirstOrDefault(d => d.CodigoOperacion == deuda.CodigoOperacion);

                if (existente != null)
                {

                    existente.DeudaCapital = deuda.DeudaCapital;
                    existente.Interes = deuda.Interes;
                    existente.GastosCobranzas = deuda.GastosCobranzas;
                    existente.SaldoDeuda = deuda.SaldoDeuda;
                    existente.Descuento = deuda.Descuento;
                    existente.MontoCobrar = deuda.MontoCobrar;
                    existente.FechaVenta = deuda.FechaVenta;
                    existente.FechaUltimoPago = deuda.FechaUltimoPago;
                    existente.Estado = deuda.Estado;
                    existente.DiasMora = deuda.DiasMora;
                    existente.NumeroFactura = deuda.NumeroFactura;
                    existente.Clasificacion = deuda.Clasificacion;
                    existente.Creditos = deuda.Creditos;
                    existente.ValorCuota = deuda.ValorCuota;
                    existente.EsActivo = true;
                    existente.Empresa = deuda.Empresa;
                    existente.MontoPonteAlDia = deuda.MontoPonteAlDia;
                    existente.Tramo = deuda.Tramo;
                    existente.UltimoPago = deuda.UltimoPago;
                    existente.CodigoOperacion = deuda.CodigoOperacion;
                    existente.MontoCobrarPartes = deuda.MontoCobrarPartes;
                    existente.FechaRegistro = DateTime.UtcNow;

                    if (string.IsNullOrEmpty(existente.IdUsuario))
                        existente.IdUsuario = deuda.IdUsuario;

                    //_dataContext.Deudas.Update(existente);
                    deudasUpdate.Add(existente);
                }
                else
                {
                    if (deuda.IdDeuda == Guid.Empty)
                        deuda.IdDeuda = Guid.NewGuid();
                    deuda.FechaRegistro = DateTime.UtcNow;
                    deuda.CodigoOperacion = deuda.CodigoOperacion;
                    deuda.EsActivo = true;
                    deudasNuevo.Add(deuda);
                    //_dataContext.Deudas.Add(deuda);
                }
            }

            var bulk = new NpgsqlBulkUploader(_dataContext);
            bulk.Update(deudasUpdate);
            bulk.Insert(deudasNuevo);

            return ("Se insertó y actualizó correctamente");
        }


        public async Task<int> ImportarDeudoresCompletoAsync()
        {
            if (!Directory.Exists(_root))
                throw new DirectoryNotFoundException($"No existe la carpeta: {_root}");

            var datosCliente = new ConcurrentDictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            var direccionCliente = new ConcurrentDictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            var files = Directory.EnumerateFiles(_root, "*.*", SearchOption.AllDirectories)
                                 .Where(f =>
                                     f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                                     f.EndsWith(".csv", StringComparison.OrdinalIgnoreCase));

            foreach (var file in files)
            {
                var upperName = Path.GetFileNameWithoutExtension(file).ToUpperInvariant();

                if (upperName.Contains("DATOSCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        if (!row.TryGetValue("CNUMEROIDENTIFICACION", out var codigo) || string.IsNullOrWhiteSpace(codigo))
                            continue;
                        datosCliente[codigo] = row; // último gana
                    }
                }
                else if (upperName.Contains("DIRECCIONCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        if (!row.TryGetValue("CNUMEROIDENTIFICACION", out var codigo) || string.IsNullOrWhiteSpace(codigo))
                            continue;
                        direccionCliente[codigo] = row; // último gana
                    }
                }
            }

            var candidatos = new List<Deudores>();

            foreach (var kv in datosCliente)
            {
                var codigo = kv.Key;
                var d = kv.Value;  
                direccionCliente.TryGetValue(codigo, out var dir); // puede no existir

                d.TryGetValue("CNUMEROIDENTIFICACION", out var idDeudor);

                d.TryGetValue("CNOMBRECOMPLETO", out var nombreCompleto);
                d.TryGetValue("NOMBRES", out var nombres);
                d.TryGetValue("APELLIDOS", out var apellidos);

                d.TryGetValue("CNOMBREEMPRESA", out var empresa);

                d.TryGetValue("REFERENCIA", out var referencia);
                d.TryGetValue("DESCRIP_TITULO", out var descripTitulo);

                var direccion =  Get(dir, "CDIRECCIONCOMPLETA") ;
                var telefono = Get(dir, "TELEFONO");
                var email = Get(dir, "CCORREOELECTRONICO");

                var codigoDeudor = Get(d, "ICODIGOCLIENTE");

                if (string.IsNullOrWhiteSpace(idDeudor))
                    continue;

                var nombre = !string.IsNullOrWhiteSpace(nombreCompleto)
                    ? nombreCompleto
                    : string.Join(" ", new[] { nombres, apellidos }.Where(s => !string.IsNullOrWhiteSpace(s)));

                var descripcion = Get(dir, "COBSERVACION");

                var entidad = new Deudores
                {
                    IdDeudor = idDeudor!.Trim(),
                    Nombre = (nombre ?? "").Trim(),
                    Direccion = direccion?.Trim(),
                    Empresa = "CRECOSCORP",
                    Telefono = telefono?.Trim(),
                    Correo = email?.Trim(),
                    Descripcion = descripcion?.Trim(),
                    CodigoDeudor = codigoDeudor?.Trim(),
                    FechaRegistro = DateTime.Now
                };

                candidatos.Add(entidad);
            }

            if (candidatos.Count == 0) return 0;

            var ids = candidatos.Select(c => c.IdDeudor).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var existentes = await _dataContext.Set<Deudores>()
                                      .Where(d => ids.Contains(d.IdDeudor))
                                      .ToDictionaryAsync(d => d.IdDeudor, StringComparer.OrdinalIgnoreCase);

            var nuevos = new List<Deudores>();

            foreach (var c in candidatos)
            {
                if (existentes.TryGetValue(c.IdDeudor, out var e))
                {
                    e.Nombre = c.Nombre;
                    e.Direccion = c.Direccion;
                    e.Empresa = c.Empresa;
                    e.Telefono = c.Telefono;
                    e.Correo = c.Correo;
                    e.Descripcion = c.Descripcion;
                    e.CodigoDeudor = string.IsNullOrWhiteSpace(e.CodigoDeudor) ? c.CodigoDeudor : e.CodigoDeudor;
                    e.FechaRegistro = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    e.IdUsuario = null;
                }
                else
                {
                    c.FechaRegistro = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    c.IdUsuario = null;
                    nuevos.Add(c);
                }
            }

            if (nuevos.Count > 0)
                await _dataContext.Set<Deudores>().AddRangeAsync(nuevos);

            return await _dataContext.SaveChangesAsync();
        }


        public async Task<int> ImportarTelefonosBasicoAsync()
        {
            if (!Directory.Exists(_root))
                throw new DirectoryNotFoundException($"No existe la carpeta: {_root}");

            var files = Directory.EnumerateFiles(_root, "*.*", SearchOption.AllDirectories)
                                 .Where(f =>
                                     (f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                                      f.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)) &&
                                     Path.GetFileNameWithoutExtension(f).ToUpperInvariant().Contains("TELEFONOCLIENTE"));

            var candidatos = new List<(string IdDeudor, string Telefono, string? Origen, string? Observacion, string? Propietario)>();
            foreach (var file in files)
            {
                foreach (var row in ReadDelimited(file))
                {
                    var idDeudor = Get(row, "CNUMEROIDENTIFICACION");
                    var numero = Get(row, "CNUMERO");
                    var prefijo = "";

                    if (string.IsNullOrWhiteSpace(idDeudor) || string.IsNullOrWhiteSpace(numero))
                        continue;

                    idDeudor = idDeudor.Trim();
                    var telefono = string.IsNullOrWhiteSpace(prefijo) ? numero : $"{prefijo}{numero}";
                    telefono = telefono.Trim();

                    var origen = Get(row, "DESCRIP_UBICACION"); // Domicilio/Laboral/etc.
                    var observacion = "[MIGRADO SSH]";
                    var propietario = "deudor";

                    candidatos.Add((idDeudor, telefono, origen, observacion, propietario));
                }
            }

            if (candidatos.Count == 0) return 0;

            var idsDeArchivo = candidatos.Select(c => c.IdDeudor)
                                         .Distinct(StringComparer.OrdinalIgnoreCase)
                                         .ToList();

            var deudoresExistentes = await _dataContext.Set<Deudores>()
                .Where(d => idsDeArchivo.Contains(d.IdDeudor))
                .Select(d => d.IdDeudor)
                .ToListAsync();

            var setDeudores = new HashSet<string>(deudoresExistentes, StringComparer.OrdinalIgnoreCase);

            var existentes = await _dataContext.Set<DeudorTelefono>()
                .Where(t => t.IdDeudor != null &&
                            idsDeArchivo.Contains(t.IdDeudor))
                .Select(t => new { t.IdDeudor, t.Telefono })
                .ToListAsync();

            string MakeKey(string id, string tel) => $"{id}|{tel}";
            var setExistentes = new HashSet<string>(
                existentes.Select(e => MakeKey(e.IdDeudor!, e.Telefono!)),
                StringComparer.OrdinalIgnoreCase
            );

            var setLote = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var nuevos = new List<DeudorTelefono>();

            foreach (var c in candidatos)
            {
                if (!setDeudores.Contains(c.IdDeudor))
                    continue; 

                var key = MakeKey(c.IdDeudor, c.Telefono);

                if (setExistentes.Contains(key))
                    continue; // ya existe en BD

                if (!setLote.Add(key))
                    continue; 

                nuevos.Add(new DeudorTelefono
                {
                    IdDeudorTelefonos = Guid.NewGuid().ToString("N"),
                    IdDeudor = c.IdDeudor,
                    Telefono = c.Telefono,
                    FechaAdicion = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                    EsValido = true,
                    Propietario = c.Propietario,
                    Origen = c.Origen,
                    Observacion = c.Observacion
                });
            }

            if (nuevos.Count == 0) return 0;

            await _dataContext.Set<DeudorTelefono>().AddRangeAsync(nuevos);
            return await _dataContext.SaveChangesAsync();
        }




        public async Task<int> ImportarDeudasBasicoAsync()
        {
            if (!Directory.Exists(_root))
                throw new DirectoryNotFoundException($"No existe la carpeta: {_root}");

            // Índices simples en memoria
            var carteraById = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            var operacionesByICodigo = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase); // ICODIGOOPERACION
            var articulosPorFactura = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);               // NUM_FACTURA
            var articulosPorCodOperacion = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);               // COD_OPERACION
            var saldos = new List<Dictionary<string, string>>();

            // 1) Leer archivos
            var files = Directory.EnumerateFiles(_root, "*.*", SearchOption.AllDirectories)
                                 .Where(f => f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                                             f.EndsWith(".csv", StringComparison.OrdinalIgnoreCase));

            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file).ToUpperInvariant();

                if (name.Contains("CARTERAASIGNADA"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        var id = Get(row, "CNUMEROIDENTIFICACION");
                        if (!string.IsNullOrWhiteSpace(id))
                            carteraById[id.Trim()] = row;
                    }
                }
                else if (name.Contains("OPERACIONESCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        // Clave: ICODIGOOPERACION
                        var iCodOp = GetAny(row, "ICODIGOOPERACION", "I_CODIGO_OPERACION", "ICODIGO_OP");
                        if (!string.IsNullOrWhiteSpace(iCodOp))
                            operacionesByICodigo[iCodOp.Trim()] = row;
                    }
                }
                else if (name.Contains("ARTICULOOPERACION"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        var factura = GetAny(row, "NUM_FACTURA", "NRO_FACTURA", "NUMEROFACTURA");
                        var codOp = GetAny(row, "COD_OPERACION");
                        var prod = GetAny(row, "DESCRIP_ARTICULO", "DESCRIPCION_ARTICULO", "PRODUCTO", "ARTICULO", "NOMBRE");

                        if (!string.IsNullOrWhiteSpace(factura) && !string.IsNullOrWhiteSpace(prod))
                        {
                            var key = factura!.Trim();
                            if (!articulosPorFactura.TryGetValue(key, out var listF)) articulosPorFactura[key] = listF = new List<string>();
                            listF.Add(prod!.Trim());
                        }

                        if (!string.IsNullOrWhiteSpace(codOp) && !string.IsNullOrWhiteSpace(prod))
                        {
                            var key = codOp!.Trim();
                            if (!articulosPorCodOperacion.TryGetValue(key, out var listO)) articulosPorCodOperacion[key] = listO = new List<string>();
                            listO.Add(prod!.Trim());
                        }
                    }
                }
                else if (name.Contains("SALDOCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                        saldos.Add(row);
                }
            }

            if (saldos.Count == 0) return 0;

            // 2) Construir entidades Deuda (básico, sin deduplicar)
            var nuevos = new List<Deuda>();
            var ahoraUtc = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

            foreach (var s in saldos)
            {
                var idDeudor = Get(s, "CNUMEROIDENTIFICACION")?.Trim();
                if (string.IsNullOrWhiteSpace(idDeudor)) continue;

                var numeroFactura = GetAny(s, "NUM_FACTURA", "NRO_FACTURA", "NUMEROFACTURA")?.Trim();
                var codOperacionSal = GetAny(s, "COD_OPERACION", "CODOPERACION")?.Trim(); // en SALDOCLIENTE

                // Buscar fila de OPERACIONES usando ICODIGOOPERACION (= COD_OPERACION de ARTICULOOPERACION)
                operacionesByICodigo.TryGetValue(codOperacionSal ?? "", out var opRow);

                // Agencia/Ciudad desde cartera
                carteraById.TryGetValue(idDeudor, out var carRow);

                // ProductoDescripcion:
                // 1) por factura si existe
                // 2) si no hay factura, por COD_OPERACION (respetando ICODIGOOPERACION = COD_OPERACION)
                string? productoDescripcion = null;
                if (!string.IsNullOrWhiteSpace(numeroFactura) &&
                    articulosPorFactura.TryGetValue(numeroFactura!, out var prodsF) &&
                    prodsF.Count > 0)
                {
                    productoDescripcion = string.Join(" || ", prodsF);
                }
                else if (!string.IsNullOrWhiteSpace(codOperacionSal) &&
                         articulosPorCodOperacion.TryGetValue(codOperacionSal!, out var prodsO) &&
                         prodsO.Count > 0)
                {
                    productoDescripcion = string.Join(" || ", prodsO);
                }

                var deuda = new Deuda
                {
                    IdDeuda = Guid.NewGuid(),
                    IdDeudor = idDeudor,
                    DeudaCapital = ToDecimal(GetAny(s, "DEUDA_CAPITAL", "CAPITAL")),
                    Interes = ToDecimal(GetAny(s, "INTERES", "MORA")),
                    GastosCobranzas = ToDecimal(GetAny(s, "GASTOS_COBRANZAS", "GASTOS_COBRO")),
                    SaldoDeuda = ToDecimal(GetAny(s, "SALDO_DEUDA", "SALDO")),
                    Descuento = ToInt(GetAny(s, "DESCUENTO")),
                    MontoCobrar = ToDecimal(GetAny(s, "MONTO_COBRAR", "TOTAL_COBRAR")),
                    FechaVenta = ToDateOnly(GetAny(s, "FECHA_VENTA", "FECHAEMISION")) ?? ToDateOnly(GetAny(opRow, "FECHA_VENTA", "FECHA_EMISION")),
                    FechaUltimoPago = ToDateOnly(GetAny(s, "FECHA_ULTIMO_PAGO")),
                    Estado = GetAny(s, "ESTADO", "ESTADO_DEUDA"),
                    DiasMora = ToInt(GetAny(s, "DIAS_MORA")),
                    NumeroFactura = numeroFactura,
                    Clasificacion = GetAny(s, "CLASIFICACION"),
                    Creditos = ToInt(GetAny(s, "CREDITOS")),
                    NumeroCuotas = ToInt(GetAny(s, "NUM_CUOTAS", "CUOTAS")),
                    TipoDocumento = GetAny(s, "TIPO_DOCUMENTO", "TIPODOC") ?? GetAny(opRow, "TIPO_DOCUMENTO", "TIPODOC"),
                    ValorCuota = ToDecimal(GetAny(s, "VALOR_CUOTA", "CUOTA")),
                    Tramo = GetAny(s, "TRAMO"),
                    UltimoPago = ToDecimal(GetAny(s, "ULTIMO_PAGO")),
                    ProductoDescripcion = productoDescripcion,
                    Agencia = GetAny(carRow, "AGENCIA", "NOM_AGENCIA"),
                    Ciudad = GetAny(carRow, "CIUDAD"),
                    Empresa = GetAny(s, "EMPRESA") ?? GetAny(opRow, "EMPRESA"),
                    CodigoEmpresa = GetAny(s, "COD_EMPRESA", "CODEMPRESA") ?? GetAny(opRow, "COD_EMPRESA", "CODEMPRESA"),
                    EsActivo = true,
                    FechaRegistro = ahoraUtc,
                    IdUsuario = null,
                    CodigoOperacion = codOperacionSal // guarda el mismo código usado para enlazar
                };

                // Calcular MontoCobrar si no vino
                if (deuda.MontoCobrar == null)
                {
                    var suma = (deuda.DeudaCapital ?? 0m) + (deuda.Interes ?? 0m) + (deuda.GastosCobranzas ?? 0m);
                    deuda.MontoCobrar = suma > 0 ? suma : deuda.SaldoDeuda;
                }

                nuevos.Add(deuda);
            }

            if (nuevos.Count == 0) return 0;

            await _dataContext.Set<Deuda>().AddRangeAsync(nuevos);
            return await _dataContext.SaveChangesAsync();

            // --------- Helpers locales mínimos ----------
            static string? GetAny(Dictionary<string, string>? row, params string[] cols)
            {
                if (row == null) return null;
                foreach (var c in cols)
                {
                    var v = Get(row, c);
                    if (!string.IsNullOrWhiteSpace(v)) return v;
                }
                return null;
            }

            static decimal? ToDecimal(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                s = s.Trim().Replace(" ", "").Replace(",", ".");
                return decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : (decimal?)null;
            }

            static int? ToInt(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                return int.TryParse(s.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var i) ? i : (int?)null;
            }

            static DateOnly? ToDateOnly(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;
                if (DateTime.TryParse(s.Trim(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt))
                    return DateOnly.FromDateTime(dt);
                return null;
            }
        }

        
        static DateTime? ToDate(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            string[] formats = { "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy", "yyyyMMdd" };
            return DateTime.TryParseExact(s.Trim(), formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var dt)
                ? dt
                : (DateTime?)null;
        }


        static DateTime? ParseLocalToUtcOrNull(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;

            // Intenta primero un parse general asumiendo hora local
            if (DateTime.TryParse(s, CultureInfo.GetCultureInfo("es-EC"),
                                  DateTimeStyles.AssumeLocal, out var dt))
                return ToUtc(dt);

            // Intenta con formatos comunes
            var formats = new[] {
                    "dd/MM/yyyy", "dd-MM-yyyy",
                    "yyyy-MM-dd",
                    "dd/MM/yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss",
                    "yyyy-MM-dd HH:mm:ss"
                };
            if (DateTime.TryParseExact(s, formats, CultureInfo.GetCultureInfo("es-EC"),
                                       DateTimeStyles.AssumeLocal, out dt))
                return ToUtc(dt);

            throw new FormatException($"Fecha inválida: '{s}'");
        }

        public void GrabarTablas()
        {
            List<ArticuloOperacionCrecos> listaGrabar = new List<ArticuloOperacionCrecos>();
            List<CarteraAsignadaCrecos> carteraGrabar = new List<CarteraAsignadaCrecos>();
            List<DatosClienteCrecos> clientesGrabar = new List<DatosClienteCrecos>();
            List<DireccionClienteCrecos> direccionClientesGrabar = new List<DireccionClienteCrecos>();
            List<OperacionesClientesCrecos> operacionesClienteGrabar = new List<OperacionesClientesCrecos>();
            List<ReferenciasPersonalesCrecos> referenciaClienteGrabar = new List<ReferenciasPersonalesCrecos>();
            List<SaldoClienteCrecos> saldoClienteCrecos = new List<SaldoClienteCrecos>();
            List<TelefonosClienteCrecos> telefonoClienteCrecos = new List<TelefonosClienteCrecos>();
            List<CuotaOperacionCrecos> CuotasOperacionCrecos = new List<CuotaOperacionCrecos>();

            List<ReciboDetalleCrecos> reciboDetalleCrecos = new List<ReciboDetalleCrecos>();
            List<ReciboPagosCrecos> reciboPagosCrecos = new List<ReciboPagosCrecos>();
            List<ReciboFormaPagoCrecos> reciboFormaPagoCrecos = new List<ReciboFormaPagoCrecos>();



            int? ToInt(string? s) => int.TryParse(s, out var v) ? v : null;

            decimal? ToDec(string? s)
            {
                if (string.IsNullOrWhiteSpace(s)) return null;

                s = s.Trim();

                // Si contiene coma pero no punto, asumimos formato latino (1.234,56)
                if (s.Contains(',') && !s.Contains('.'))
                {
                    // Cambia el separador decimal
                    s = s.Replace(".", "").Replace(",", ".");
                }
                else if (s.Contains('.') && s.IndexOf('.') != s.LastIndexOf('.'))
                {
                    // Si hay más de un punto (por miles y decimales)
                    int lastDot = s.LastIndexOf('.');
                    s = s.Remove(lastDot).Replace(".", "") + "." + s.Substring(lastDot + 1);
                }

                return decimal.TryParse(s, System.Globalization.NumberStyles.Any,
                                        System.Globalization.CultureInfo.InvariantCulture, out var v)
                       ? Math.Round(v, 2)
                       : null;
            }

          

            if (!Directory.Exists(_root))
                throw new DirectoryNotFoundException($"No existe la carpeta: {_root}");

            var files = Directory.EnumerateFiles(_root, "*.*", SearchOption.AllDirectories)
                                .Where(f =>
                                    f.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                                    f.EndsWith(".csv", StringComparison.OrdinalIgnoreCase));

            foreach (var file in files)
            {
                var upperName = Path.GetFileNameWithoutExtension(file).ToUpperInvariant();


                if (upperName.Contains("CUOTAOPERACION"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        CuotasOperacionCrecos.Add(new CuotaOperacionCrecos()
                        {
                            CodOperacion = row.GetValueOrDefault("COD_OPERACION") ?? "",
                            CodCuota = row.GetValueOrDefault("COD_CUOTA") ?? "",
                            NumeroCuota = (int)ToInt(row.GetValueOrDefault("NUMERO_CUOTA") ?? "0"),
                            FechaVencimiento = ToDate(row.GetValueOrDefault("FECHA_VENCIMIENTO")),
                            FechaCorte = ToDate(row.GetValueOrDefault("FECHA_CORTE")),
                            FechaUltimoPago = row.GetValueOrDefault("FECHA_ULTIMO_PAGO") == "" ? DateTime.Now : DateTime.Parse(row.GetValueOrDefault("FECHA_ULTIMO_PAGO")),
                            DFechaPostergacion = ToDate(row.GetValueOrDefault("DFECHAPOSTERGACION")),
                            CodEstadoCuota = row.GetValueOrDefault("COD_ESTADO_CUOTA"),
                            DescEstadoOperacion = row.GetValueOrDefault("DESC_ESTADO_OPERACION"),
                            TasaMora = ToDec(row.GetValueOrDefault("TASA_MORA")),
                            CodEstadoRegistro = row.GetValueOrDefault("COD_ESTADO_REGISTRO"),
                            DesEstadoRegistro = row.GetValueOrDefault("DES_ESTADO_REGISTRO"),
                            IValorTotalCuota = ToDec(row.GetValueOrDefault("IVALORTOTALCUOTA")),
                            IValorCuota = ToDec(row.GetValueOrDefault("IVALORCUOTA")),
                            ValorCapitalInteres = ToDec(row.GetValueOrDefault("VALOR_CAPITAL_INTERES")),
                            ValorCargos = ToDec(row.GetValueOrDefault("VALOR_CARGOS")),
                            ValorOtrosCargos = ToDec(row.GetValueOrDefault("VALOR_OTROS_CARGOS")),
                        });
                    }
                }


                if (upperName.Contains("ARTICULOOPERACION"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        listaGrabar.Add(new ArticuloOperacionCrecos()
                        {
                            CANTIDAD = int.Parse(row.GetValueOrDefault("CANTIDAD")),
                            ISECUENCIAL = int.Parse(row.GetValueOrDefault("ISECUENCIAL")),
                            COD_PRODUCTO = row.GetValueOrDefault("COD_PRODUCTO"),
                            COD_OPERACION =row.GetValueOrDefault("COD_OPERACION"),
                            DESC_PRODUCTO = row.GetValueOrDefault("DESC_PRODUCTO"),
                            OBSERVACION = ""
                        });
                    }
                }

                if (upperName.Contains("RECIBOPAGO"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        reciboPagosCrecos.Add(new ReciboPagosCrecos()
                        {
                            CodRecibo = row.GetValueOrDefault("COD_RECIBO"),
                            CestadoRegistro = row.GetValueOrDefault("CESTADO_REGISTRO"),
                            CodEmpresa = row.GetValueOrDefault("COD_EMPRESA"),
                            DescripcEmpresa = row.GetValueOrDefault("DESCRIPC_EMPRESA"),
                            CodUNegocio = row.GetValueOrDefault("COD_UNEGOCIO"),
                            DescripcUNegocio = row.GetValueOrDefault("DESCRIPC_UNEGOCIO"),
                            CodTCartera = row.GetValueOrDefault("COD_TCARTERA"),
                            DescripcTCartera = row.GetValueOrDefault("DESCRIPC_TCARTERA"),
                            CodOficina = row.GetValueOrDefault("COD_OFICINA"),
                            CDescripcionOficina = row.GetValueOrDefault("CDESCRIPCION_OFICINA"),
                            NumIdentificacion = row.GetValueOrDefault("NUM_IDENTIFICACION"),
                            CodPagoReferencial = row.GetValueOrDefault("COD_PAGO_REFERENCIAL"),
                            CodMoneda = row.GetValueOrDefault("COD_MONEDA"),
                            DescripcMoneda = row.GetValueOrDefault("DESCRIPC_MONEDA"),
                            CodTPago = row.GetValueOrDefault("COD_TPAGO"),
                            DescripcTPago = row.GetValueOrDefault("DESCRIPC_TPAGO"),
                            CodCaja = row.GetValueOrDefault("COD_CAJA"),
                            DescripcCaja = row.GetValueOrDefault("DESCRIPC_CAJA"),
                            CodGestor = row.GetValueOrDefault("COD_GESTOR"),
                            DescripcGestor = row.GetValueOrDefault("DESCRIPC_GESTOR"),
                            CodTRecibo = row.GetValueOrDefault("COD_TRECIBO"),
                            DescripcTRecibo = row.GetValueOrDefault("DESCRIPC_TRECIBO"),
                            FechaPago = ParseLocalToUtcOrNull(row.GetValueOrDefault("FECHA_PAGO")) ,
                            DFechaReverso = ParseLocalToUtcOrNull(row.GetValueOrDefault("DFECHAREVERSO")),
                            Monto = ToDec(row.GetValueOrDefault("MONTO")),
                            Cambio = ToDec(row.GetValueOrDefault("CAMBIO")),
                            NombreArchivo = upperName
                        });
                    }
                }


                if (upperName.Contains("RECIBODETALLE_"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        reciboDetalleCrecos.Add(new ReciboDetalleCrecos
                        {
                            // IReciboDetalle se genera en la entidad (Guid). Si lo necesitas del archivo, así:
                            IReciboDetalle = row.GetValueOrDefault("IRECIBODETALLE"),
                            CodRecibo = row.GetValueOrDefault("COD_RECIBO"),
                            ICodigoOperacion = row.GetValueOrDefault("ICODIGOOPERACION"),
                            NumCuota = ToInt(row.GetValueOrDefault("NUM_CUOTA")),
                            CodRubro = row.GetValueOrDefault("COD_RUBRO"),
                            CDescripcionRubro = row.GetValueOrDefault("CDESCRIPCION_RUBRO"),
                            ValorRecibo = ToDec(row.GetValueOrDefault("VALOR_RECIBO")),
                            NombreArchivo = upperName
                        });
                    }
                }

                if (upperName.Contains("RECIBOFORMAPAGO") )
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        reciboFormaPagoCrecos.Add(new ReciboFormaPagoCrecos
                        {
                            CodReciboFormaPago = row.GetValueOrDefault("COD_RECIBO_FORMAPAGO"), // si viene en el archivo
                            CodRecibo = row.GetValueOrDefault("COD_RECIBO"),
                            CodFormaPago = row.GetValueOrDefault("COD_FORMA_PAGO"),
                            DescripcFPago = row.GetValueOrDefault("DESCRIPC_FPAGO"),
                            CodInsFinanciera = row.GetValueOrDefault("COD_INS_FINANCIERA"),
                            CDescripcionInstitucionFinanciera = row.GetValueOrDefault("CDESCRIPCION_INSTITUCION_FINANCIERA"),
                            NumCuenta = row.GetValueOrDefault("NUM_CUENTA"),
                            NumDocumento = row.GetValueOrDefault("NUM_DOCUMENTO"),
                            CNombreCuentaCorrentista = row.GetValueOrDefault("CNOMBRECUENTACORRENTISTA"),
                            CCedulaCuentaCorrentista = row.GetValueOrDefault("CCEDULACUENTACORRENTISTA"),
                            DFechaCobroDocumento = ParseLocalToUtcOrNull(row.GetValueOrDefault("DFECHACOBRODOCUMENTO")),
                            CodMoneda = row.GetValueOrDefault("COD_MONEDA"),
                            DescripcMoneda = row.GetValueOrDefault("DESCRIPC_MONEDA"),
                            CodMotivo = row.GetValueOrDefault("COD_MOTIVO"),
                            DescripcMotivo = row.GetValueOrDefault("DESCRIPC_MOTIVO"),
                            IValor = ToDec(row.GetValueOrDefault("IVALOR")),
                            NombreArchivo = upperName
                        });
                    }
                }


                if (upperName.Contains("CARTERAASIGNADA"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        carteraGrabar.Add(new CarteraAsignadaCrecos()
                        {
                            COD_EMPRESA = row.GetValueOrDefault("COD_EMPRESA"),
                            EMPRESA = row.GetValueOrDefault("EMPRESA"),
                            COD_UNIDAD_NEGOCIO = row.GetValueOrDefault("COD_UNIDAD_NEGOCIO"),
                            UNIDAD_NEGOCIO = row.GetValueOrDefault("UNIDAD_NEGOCIO"),
                            COD_TIPO_CARTERA = row.GetValueOrDefault("COD_TIPO_CARTERA"),
                            TIPO_CARTERA = row.GetValueOrDefault("TIPO_CARTERA"),
                            IMES = ToInt(row.GetValueOrDefault("IMES")),
                            IANO = ToInt(row.GetValueOrDefault("IANO")),
                            CNUMEROIDENTIFICACION = row.GetValueOrDefault("CNUMEROIDENTIFICACION"),
                            CNOMBRECOMPLETO = row.GetValueOrDefault("CNOMBRECOMPLETO"),
                            COD_TIPO_GESTOR = row.GetValueOrDefault("COD_TIPO_GESTOR"),
                            CDESCRIPCION = row.GetValueOrDefault("CDESCRIPCION"),
                            BCUOTAIMPAGA = row.GetValueOrDefault("BCUOTAIMPAGA"),
                            DIAS_MORA = ToInt(row.GetValueOrDefault("DIAS_MORA")),
                            DFECHAVENCIMIENTO = (row.GetValueOrDefault("DFECHAVENCIMIENTO")),
                            IVALORDEUDATOTAL = ToDec(row.GetValueOrDefault("IVALORDEUDATOTAL")),
                            ICICLOCORTE = ToInt(row.GetValueOrDefault("ICICLOCORTE")),
                            COD_PAIS = row.GetValueOrDefault("COD_PAIS"),
                            PAIS = row.GetValueOrDefault("PAIS"),
                            COD_PROVINCIA = row.GetValueOrDefault("COD_PROVINCIA"),
                            PROVINCIA = row.GetValueOrDefault("PROVINCIA"),
                            COD_CANTON = row.GetValueOrDefault("COD_CANTON"),
                            CANTON = row.GetValueOrDefault("CANTON"),
                            COD_ZONA = row.GetValueOrDefault("COD_ZONA"),
                            ZONA = row.GetValueOrDefault("ZONA"),
                            COD_BARRIO = row.GetValueOrDefault("COD_BARRIO"),
                            BARRIO = row.GetValueOrDefault("BARRIO"),
                            COD_GESTOR = row.GetValueOrDefault("COD_GESTOR"),
                            GESTOR = row.GetValueOrDefault("GESTOR"),
                            CODIGOCLIENTE = row.GetValueOrDefault("CODIGOCLIENTE"),
                            NOMBREARCHIVO = upperName
                        });
                    }
                }

                if (upperName.Contains("DATOSCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        clientesGrabar.Add(new DatosClienteCrecos()
                        {
                            CCODIGOTIPOPERSONA = row.GetValueOrDefault("CCODIGOTIPOPERSONA"),
                            TIPO_PERSONA = row.GetValueOrDefault("TIPO_PERSONA"),
                            COD_TIPO_IDENTIF = row.GetValueOrDefault("COD_TIPO_IDENTIF"),
                            DESCRIP_TIPO_IDENTIF = row.GetValueOrDefault("DESCRIP_TIPO_IDENTIFIC") ?? row.GetValueOrDefault("DESCRIP_TIPO_IDENTIF"),
                            CNUMEROIDENTIFICACION = row.GetValueOrDefault("CNUMEROIDENTIFICACION"),
                            CNOMBRECOMPLETO = row.GetValueOrDefault("CNOMBRECOMPLETO"),
                            CEXENTOIMPUESTO = row.GetValueOrDefault("CEXENTOIMPUESTO"),
                            CPRIMERNOMBRE = row.GetValueOrDefault("CPRIMERNOMBRE"),
                            CSEGUNDONOMBRE = row.GetValueOrDefault("CSEGUNDONOMBRE"),
                            CAPELLIDOPATERNO = row.GetValueOrDefault("CAPELLIDOPATERNO"),
                            CAPELLIDOMATERNO = row.GetValueOrDefault("CAPELLIDOMATERNO"),
                            CCODIGOESTADOCIVIL = row.GetValueOrDefault("CCODIGOESTADOCIVIL"),
                            DESCRIP_ESTADO_CIVIL = row.GetValueOrDefault("DESCRIP_ESTADO_CIVIL"),
                            CCODIGOSEXO = row.GetValueOrDefault("CCODIGOSEXO"),
                            DESCRIP_SEXO = row.GetValueOrDefault("DESCRIP_SEXO"),
                            CCODIGOPAIS = row.GetValueOrDefault("CCODIGOPAIS"),
                            DESCRIP_PAIS = row.GetValueOrDefault("DESCRIP_PAIS"),
                            CCODIGOCIUDADNACIMIENTO = row.GetValueOrDefault("CCODIGOCIUDADNACIMIENTO"),
                            DESCRIP_CIUDAD_NACIMIENTO = row.GetValueOrDefault("DESCRIP_CIUDAD_NACIMIENTO"),
                            CCODIGONACIONALIDAD = row.GetValueOrDefault("CCODIGONACIONALIDAD"),
                            DESCRIP_NACIONALIDAD = row.GetValueOrDefault("DESCRIP_NACIONALIDAD"),
                            DFECHANACIMIENTO = row.GetValueOrDefault("DFECHANACIMIENTO"),
                            INUMEROCARGA = ToInt(row.GetValueOrDefault("INUMEROCARGA")),
                            CSEPARACIONBIEN = row.GetValueOrDefault("CSEPARACIONBIEN"),
                            CCODIGONIVELEDUCACION = row.GetValueOrDefault("CCODIGONIVELEDUCACION"),
                            DESCRIP_NIVEL_EDUC = row.GetValueOrDefault("DESCRIP_NIVEL_EDUC"),
                            CCODIGOTITULO = row.GetValueOrDefault("CCODIGOTITULO"),
                            DESCRIP_TITULO = row.GetValueOrDefault("DESCRIP_TITULO"),
                            CRAZONCOMERCIAL = row.GetValueOrDefault("CRAZONCOMERCIAL"),
                            CNOMBREEMPRESA = row.GetValueOrDefault("CNOMBREEMPRESA"),
                            CCODIGOCARGO = row.GetValueOrDefault("CCODIGOCARGO"),
                            DESCRIP_CARGO = row.GetValueOrDefault("DESCRIP_CARGO"),
                            DFECHAINGRESOLAB = row.GetValueOrDefault("DFECHAINGRESOLAB"),
                            IINGRESO = ToDec(row.GetValueOrDefault("IINGRESO")),
                            IEGRESO = ToDec(row.GetValueOrDefault("IEGRESO")),
                            ICODIGOCLIENTE = row.GetValueOrDefault("ICODIGOCLIENTE"),
                            ISCORE = ToInt(row.GetValueOrDefault("ISCORE")),
                        });
                    }


                   
                }

                if (upperName.Contains("DIRECCIONCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        direccionClientesGrabar.Add(new DireccionClienteCrecos()
                        {
                            CNUMEROIDENTIFICACION = row.GetValueOrDefault("CNUMEROIDENTIFICACION"),
                            CODIGO_UBICACION = row.GetValueOrDefault("CODIGO_UBICACION"),
                            DESCRIP_UBICACION = row.GetValueOrDefault("DESCRIP_UBICACION"),
                            COD_PAIS = row.GetValueOrDefault("COD_PAIS"),
                            DESCRIP_PAIS = row.GetValueOrDefault("DESCRIP_PAIS"),
                            COD_PROVINCIA = row.GetValueOrDefault("COD_PROVINCIA"),
                            DESCRIP_PROVINCIA = row.GetValueOrDefault("DESCRIP_PROVINCIA"),
                            COD_CANTON = row.GetValueOrDefault("COD_CANTON"),
                            DESCRIP_CANTON = row.GetValueOrDefault("DESCRIP_CANTON"),
                            COD_PARROQUIA = row.GetValueOrDefault("COD_PARROQUIA"),
                            DESCRIP_PARROQUIA = row.GetValueOrDefault("DESCRIP_PARROQUIA"),
                            COD_BARRIO = row.GetValueOrDefault("COD_BARRIO"),
                            DESCRIP_BARRIO = row.GetValueOrDefault("DESCRIP_BARRIO"),
                            CDIRECCIONCARGA = row.GetValueOrDefault("CDIRECCIONCARGA"),
                            COBSERVACION = row.GetValueOrDefault("COBSERVACION"),
                            CDIRECCIONCOMPLETA = row.GetValueOrDefault("CDIRECCIONCOMPLETA"),
                            CCASILLA = row.GetValueOrDefault("CCASILLA"),
                            CCORREOELECTRONICO = row.GetValueOrDefault("CCORREOELECTRONICO"),
                            COD_TIPO_ESPACIO = row.GetValueOrDefault("COD_TIPO_ESPACIO"),
                            DESCRIP_TIPO_ESPACIO = row.GetValueOrDefault("DESCRIP_TIPO_ESPACIO"),
                            INUMEROESPACIO = ToInt(row.GetValueOrDefault("INUMEROESPACIO")),
                            INUMEROSOLAR = ToInt(row.GetValueOrDefault("INUMEROSOLAR")),
                            CCALLEPRINCIPAL = row.GetValueOrDefault("CCALLEPRINCIPAL"),
                            CNUMEROCALLE = row.GetValueOrDefault("CNUMEROCALLE"),
                            CALLE_SECUND = row.GetValueOrDefault("CALLE_SECUND"),
                            CALLE_SECUND_2 = row.GetValueOrDefault("CALLE_SECUND_2"),
                            CNUMERO_SOLAR = row.GetValueOrDefault("CNUMERO_SOLAR"),
                            COD_TIPO_NUMERACION = row.GetValueOrDefault("COD_TIPO_NUMERACION"),
                            DESCRIP_TIPO_NUMERACION = row.GetValueOrDefault("DESCRIP_TIPO_NUMERACION"),
                            COD_INDICADOR_POSICION = row.GetValueOrDefault("COD_INDICADOR_POSICION"),
                            DESCRIP_IND_POSICION = row.GetValueOrDefault("DESCRIP_IND_POSICION"),
                            NOMBRE_EDIFICIO = row.GetValueOrDefault("NOMBRE_EDIFICIO"),
                            CNUMEROPISO = row.GetValueOrDefault("CNUMEROPISO"),
                            PISO_BLOQUE = row.GetValueOrDefault("PISO_BLOQUE"),
                            COFICINA_DEPARTAMENTO = row.GetValueOrDefault("COFICINA_DEPARTAMENTO"),
                            INDICADOR_PRINCIPAL = row.GetValueOrDefault("INDICADOR_PRINCIPAL"),
                            COD_T_PROPIEDAD = row.GetValueOrDefault("COD_T_PROPIEDAD"),
                            DESCRIP_TIPO_PROPIEDAD = row.GetValueOrDefault("DESCRIP_TIPO_PROPIEDAD"),
                            AÑO_ANTIGUEDAD = ToInt(row.GetValueOrDefault("AÑO_ANTIGUEDAD")),
                            MES_ANTIGUEDAD = ToInt(row.GetValueOrDefault("MES_ANTIGUEDAD")),
                            DIAS_ANTIGUEDAD = ToInt(row.GetValueOrDefault("DIAS_ANTIGUEDAD")),
                        });
                    }
                }

                if (upperName.Contains("OPERACIONESCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        operacionesClienteGrabar.Add(new OperacionesClientesCrecos()
                        {
                            ICODIGOOPERACION = row.GetValueOrDefault("ICODIGOOPERACION"),
                            COD_OFICINA = row.GetValueOrDefault("COD_OFICINA"),
                            DESC_OFICINA = row.GetValueOrDefault("DESC_OFICINA"),
                            N_IDENTIFICACION = row.GetValueOrDefault("N_IDENTIFICACION"),
                            NUM_FACTURA = row.GetValueOrDefault("NUM_FACTURA"),
                            COD_MONEDA = row.GetValueOrDefault("COD_MONEDA"),
                            DESC_MONEDA = row.GetValueOrDefault("DESC_MONEDA"),
                            COD_PROD_FINANCIERO = row.GetValueOrDefault("COD_PROD_FINANCIERO"),
                            DES_PROD_FINANCIERO = row.GetValueOrDefault("DES_PROD_FINANCIERO"),
                            ICODIGO_OPERACION_NEGOCIACION = row.GetValueOrDefault("ICODIGO_OPERACION_NEGOCIACION"),
                            NUM_CUOTAS = ToInt(row.GetValueOrDefault("NUM_CUOTAS")),
                            TASA_INTERES = ToDec(row.GetValueOrDefault("TASA_INTERES")),
                            FECHA_FACTURA = (row.GetValueOrDefault("FECHA_FACTURA")),
                            FECHA_ULTIMO_VENCIMIENTO = (row.GetValueOrDefault("FECHA_ULTIMO_VENCIMIENTO")),
                            FECHA_ULTMO_PAGO = (row.GetValueOrDefault("FECHA_ULTMO_PAGO")),
                            MONTO_CREDITO = ToDec(row.GetValueOrDefault("MONTO_CREDITO")),
                            VALOR_FINANCIAR = ToDec(row.GetValueOrDefault("VALOR_FINANCIAR")),
                            NUMERO_SOLICITUD = row.GetValueOrDefault("NUMERO_SOLICITUD"),
                            COD_T_OPERACION = row.GetValueOrDefault("COD_T_OPERACION"),
                            DESC_T_OPERACION = row.GetValueOrDefault("DESC_T_OPERACION"),
                            COD_T_CREDITO = row.GetValueOrDefault("COD_T_CREDITO"),
                            DESC_T_CREDITO = row.GetValueOrDefault("DESC_T_CREDITO"),
                            COD_ESTADO_OPERACION = row.GetValueOrDefault("COD_ESTADO_OPERACION"),
                            DESC_ESTADO_OPERACION = row.GetValueOrDefault("DESC_ESTADO_OPERACION"),
                            SECUENC_CUPO = ToInt(row.GetValueOrDefault("SECUENC_CUPO")),
                            ESTADO_REGISTRO = row.GetValueOrDefault("ESTADO_REGISTRO"),
                            DES_ESTADO_REGISTRO = row.GetValueOrDefault("DES_ESTADO_REGISTRO"),
                            COD_VENDEDOR = row.GetValueOrDefault("COD_VENDEDOR"),
                            DESC_VENDEDOR = row.GetValueOrDefault("DESC_VENDEDOR"),
                        });
                    }
                }


                if (upperName.Contains("REFERENCIASPERSONALES"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        var nueva = new ReferenciasPersonalesCrecos()
                        {
                            NumIdentificacion = row.GetValueOrDefault("NUM_IDENTIFICACION"),
                            NombreCompleto = row.GetValueOrDefault("CNOMBRECOMPLETO"),
                            CodTipoIdentRef = row.GetValueOrDefault("COD_TIPO_IDENT_REF"),
                            DescripTipoIdentific = row.GetValueOrDefault("DESCRIPC_TIPO_IDENTIFIC"),
                            NumIdentificRef = row.GetValueOrDefault("NUM_IDENTIFIC_REF"),
                            NombreReferencia = row.GetValueOrDefault("NOMBRE_REFERENCIA"),
                            CodPaisRef = row.GetValueOrDefault("COD_PAIS_REF"),
                            DescripPais = row.GetValueOrDefault("DESCRIP_PAIS"),
                            CodProvinciaRef = row.GetValueOrDefault("COD_PROVINCIA_REF"),
                            DescripProvincia = row.GetValueOrDefault("DESCRIP_PROVINCIA"),
                            CodCantonRef = row.GetValueOrDefault("COD_CANTON_REF"),
                            DescripCanton = row.GetValueOrDefault("DESCRIP_CANTON"),
                            CodTipoVinculoRef = row.GetValueOrDefault("COD_TIPO_VINCULO_REF"),
                            DescripVinculo = row.GetValueOrDefault("DESCRIP_VINCULO"),
                            DireccionRef = row.GetValueOrDefault("DIRECCION_REF"),
                            NumeroReferencia = row.GetValueOrDefault("NUMERO_REFERENCIA"),
                        };

                        bool existe = referenciaClienteGrabar.Any(r =>
                            r.NumIdentificacion == nueva.NumIdentificacion &&
                            r.NombreCompleto == nueva.NombreCompleto &&
                            r.CodTipoIdentRef == nueva.CodTipoIdentRef &&
                            r.DescripTipoIdentific == nueva.DescripTipoIdentific &&
                            r.NumIdentificRef == nueva.NumIdentificRef &&
                            r.NombreReferencia == nueva.NombreReferencia &&
                            r.CodPaisRef == nueva.CodPaisRef &&
                            r.DescripPais == nueva.DescripPais &&
                            r.CodProvinciaRef == nueva.CodProvinciaRef &&
                            r.DescripProvincia == nueva.DescripProvincia &&
                            r.CodCantonRef == nueva.CodCantonRef &&
                            r.DescripCanton == nueva.DescripCanton &&
                            r.CodTipoVinculoRef == nueva.CodTipoVinculoRef &&
                            r.DescripVinculo == nueva.DescripVinculo &&
                            r.DireccionRef == nueva.DireccionRef &&
                            r.NumeroReferencia == nueva.NumeroReferencia
                        );

                        if (!existe)
                        {
                            referenciaClienteGrabar.Add(nueva);
                        }
                    }
                }

                if (upperName.Contains("SALDOCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        saldoClienteCrecos.Add(new SaldoClienteCrecos()
                        {
                            COD_EMPRESA = row.GetValueOrDefault("COD_EMPRESA"),
                            DESCP_EMPRESA = row.GetValueOrDefault("DESCP_EMPRESA"),
                            COD_U_NEGOCIO = row.GetValueOrDefault("COD_U_NEGOCIO"),
                            DESC_U_NEGOCIO = row.GetValueOrDefault("DESC_U_NEGOCIO"),
                            COD_CARTERA = row.GetValueOrDefault("COD_CARTERA"),
                            DESCRIP_CARTERA = row.GetValueOrDefault("DESCRIP_CARTERA"),
                            COD_GESTOR = row.GetValueOrDefault("COD_GESTOR"),
                            DESC_GESTOR = row.GetValueOrDefault("DESC_GESTOR"),
                            IMES = ToInt(row.GetValueOrDefault("IMES")),
                            IANO = ToInt(row.GetValueOrDefault("IANO")),
                            COD_OFICINA = row.GetValueOrDefault("COD_OFICINA"),
                            CDESCRIPCION_OFICINA = row.GetValueOrDefault("CDESCRIPCION_OFICINA"),
                            COD_TCREDITO = row.GetValueOrDefault("COD_TCREDITO"),
                            DESCRIP_TCREDITO = row.GetValueOrDefault("DESCRIP_TCREDITO"),
                            CNUMEROIDENTIFICACION = row.GetValueOrDefault("CNUMEROIDENTIFICACION"),
                            COD_OPERACION = row.GetValueOrDefault("COD_OPERACION"),
                            CNUMEROTARJETA = row.GetValueOrDefault("CNUMEROTARJETA"),
                            CICLO_CORTE = row.GetValueOrDefault("CICLO_CORTE"),
                            DESC_CICLOCORTE = row.GetValueOrDefault("DESC_CICLOCORTE"),
                            DIAS_VENCIDOS = ToInt(row.GetValueOrDefault("DIAS_VENCIDOS")),
                            ITRAMO = ToInt(row.GetValueOrDefault("ITRAMO")),
                            CDESCRIPCIONTRAMO = row.GetValueOrDefault("CDESCRIPCIONTRAMO"),
                            FECHA_MAX_PAGO = row.GetValueOrDefault("FECHA_MAX_PAGO"),
                            VALOR_DEUDA = ToDec(row.GetValueOrDefault("VALOR_DEUDA")),
                            VALOR_PAGO_MINIMO = ToDec(row.GetValueOrDefault("VALOR_PAGO_MINIMO")),
                            VALOR_CORRIENTE = ToDec(row.GetValueOrDefault("VALOR_CORRIENTE")),
                            VALOR_VENCIDO = ToDec(row.GetValueOrDefault("VALOR_VENCIDO")),
                            VALOR_POR_VENCER = ToDec(row.GetValueOrDefault("VALOR_POR_VENCER")),
                            VALOR_MORA = ToDec(row.GetValueOrDefault("VALOR_MORA")),
                            VALOR_GESTION = ToDec(row.GetValueOrDefault("VALOR_GESTION")),
                            VALOR_VENCIDO_CORTEANTERIOR = ToDec(row.GetValueOrDefault("VALOR_VENCIDO_CORTEANTERIOR")),
                            PRIMERA_CUOTA_VENCIDA = row.GetValueOrDefault("PRIMERA_CUOTA_VENCIDA"),
                            NEGOCIACION_ACTIVA = row.GetValueOrDefault("NEGOCIACION_ACTIVA"),
                            DFECHAEJECUCION = row.GetValueOrDefault("DFECHAEJECUCION"),
                            FECHA_INGRESO = row.GetValueOrDefault("FECHA_INGRESO"),
                            CALIFICACION_CLIENTE = row.GetValueOrDefault("CALIFICACION_CLIENTE"),
                            F_ULTIMO_CORTE = row.GetValueOrDefault("F_ULTIMO_CORTE"),
                            FECHA_ULT_PAGO = row.GetValueOrDefault("FECHA_ULT_PAGO"),
                            VAL_ULT_PAGO = ToDec(row.GetValueOrDefault("VAL_ULT_PAGO")),
                            VALOR_PAGO_MINIMO_ACTUALIZADO = ToDec(row.GetValueOrDefault("VALOR_PAGO_MINIMO_ACTUALIZADO")),
                            CODIGOCLIENTE = row.GetValueOrDefault("CODIGOCLIENTE"),
                            NOMBRE_ARCHIVO = upperName
                        });
                    }
                }

                if (upperName.Contains("TELEFONOCLIENTE"))
                {
                    foreach (var row in ReadDelimited(file))
                    {
                        telefonoClienteCrecos.Add(new TelefonosClienteCrecos()
                        {
                            ISECUENCIA = ToInt(row.GetValueOrDefault("ISECUENCIA")) ?? 0, // si es PK autoincrement, quítalo
                            CNumeroIdentificacion = row.GetValueOrDefault("CNUMEROIDENTIFICACION"),
                            CodUbicacion = row.GetValueOrDefault("COD_UBICACION"),
                            DescripUbicacion = row.GetValueOrDefault("DESCRIP_UBICACION"),
                            CodTipoTelefono = row.GetValueOrDefault("COD_TIPO_TELEFONO"),
                            TipoTelefono = row.GetValueOrDefault("TIPO_TELEFONO"),
                            CNumero = row.GetValueOrDefault("CNUMERO"),
                            CPrefijo = row.GetValueOrDefault("CPREFIJO"),
                        });
                    }
                }
               
            }

            _dataContext.Database.ExecuteSqlRaw(@"
                TRUNCATE TABLE
                  temp_crecos.""ArticuloOperacionCrecos"",
                  temp_crecos.""CarteraAsignadaCrecos"",
                  temp_crecos.""DatosClienteCrecos"",
                  temp_crecos.""DireccionClienteCrecos"",
                  temp_crecos.""OperacionesClientesCrecos"",
                  temp_crecos.""SaldoClienteCrecos"",
                  temp_crecos.""CuotasOperacionCrecos"",
                  temp_crecos.""TelefonosClienteCrecos"",
                  temp_crecos.""ReciboDetalleCrecos"",
                  temp_crecos.""ReciboFormaPagoCrecos"",
                  temp_crecos.""ReciboPagosCrecos""
                RESTART IDENTITY CASCADE;");

            var bulk = new NpgsqlBulkUploader(_dataContext);

            bulk.Insert(listaGrabar);
            bulk.Insert(carteraGrabar);
            bulk.Insert(clientesGrabar);
            bulk.Insert(direccionClientesGrabar);
            bulk.Insert(operacionesClienteGrabar);
            bulk.Insert(referenciaClienteGrabar);
            bulk.Insert(saldoClienteCrecos);
            bulk.Insert(telefonoClienteCrecos);
            bulk.Insert(CuotasOperacionCrecos);

            bulk.Insert(reciboDetalleCrecos);
            bulk.Insert(reciboPagosCrecos);
            bulk.Insert(reciboFormaPagoCrecos);
            string ss = "";

        }


        // =================== LECTOR TXT/CSV ROBUSTO ===================
        private static IEnumerable<Dictionary<string, string>> ReadDelimited(string path)
        {
            // Intentamos con UTF-8 y Latin1 (algunos TXT vienen ISO-8859-1)
            foreach (var enc in new[] { new UTF8Encoding(false), Encoding.GetEncoding("ISO-8859-1") })
            {
                try
                {
                    using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var sr = new StreamReader(fs, enc, detectEncodingFromByteOrderMarks: true);

                    string? headerLine = sr.ReadLine();
                    if (string.IsNullOrEmpty(headerLine)) yield break;

                    var delimiter = DetectDelimiter(headerLine);

                    // Reiniciar para que CsvHelper lea desde el inicio
                    fs.Seek(0, SeekOrigin.Begin);
                    using var sr2 = new StreamReader(fs, enc, detectEncodingFromByteOrderMarks: true);

                    var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = delimiter,
                        BadDataFound = null,
                        MissingFieldFound = null,
                        HeaderValidated = null,
                        IgnoreBlankLines = true,
                        TrimOptions = TrimOptions.Trim
                    };

                    using var csv = new CsvReader(sr2, cfg);
                    if (!csv.Read() || !csv.ReadHeader()) yield break;

                    var headers = csv.HeaderRecord!.Select(Normalize).ToArray();

                    while (csv.Read())
                    {
                        var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        for (int i = 0; i < headers.Length; i++)
                        {
                            string h = headers[i];
                            string v = csv.TryGetField(i, out string? value) ? (value ?? "") : "";
                            row[h] = v.Trim();
                        }
                        yield return row;
                    }

                    yield break; // si llegó aquí, no probamos el siguiente encoding
                }
                finally
                {
                    // probar siguiente encoding
                }
            }
        }

        private static string DetectDelimiter(string headerLine)
        {
            // Contamos ocurrencias de delimitadores comunes
            var candidates = new[] { "\t", "|", ";", "," };
            string best = "\t"; // preferimos TAB (caso típico en TXT)
            int bestCount = -1;

            foreach (var d in candidates)
            {
                int count = headerLine.Split(new[] { d }, StringSplitOptions.None).Length - 1;
                if (count > bestCount)
                {
                    bestCount = count;
                    best = d;
                }
            }
            return best;
        }

        // =================== Helpers ===================
        private static string Normalize(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var x = s.Trim().ToUpperInvariant().Replace(" ", "");
            x = x.Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U").Replace("Ñ", "N");
            return x;
        }

        private static string? Get(Dictionary<string, string>? row, string columnName)
        {
            if (row == null) return null;
            var key = Normalize(columnName);
            return row.TryGetValue(key, out var v) ? v : null;
        }
    }
}
