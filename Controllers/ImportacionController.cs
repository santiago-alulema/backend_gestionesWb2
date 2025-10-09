using gestiones_backend.Context;
using gestiones_backend.Entity;
using gestiones_backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportacionController : ControllerBase
    {
        private readonly DeudoresImportService svc;
        private readonly DataContext _dataContext;

        public ImportacionController(DeudoresImportService _svc, DataContext datacontex)
        {
            svc = _svc;
            _dataContext = datacontex;
        }


        [HttpPost("deudores/insertar-deudores")]
        public async Task<IActionResult> ImportarDeudas()
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
                                COALESCE(ROUND((MAX(t.valorliquidacion) / NULLIF(MAX(scc.""VALOR_DEUDA""),0)) * 100, 0), 0)::int
                                                                                                               AS ""Descuento"",
                                ROUND(MAX(scc.""VALOR_DEUDA""), 2)::numeric(18,2)                              AS ""DeudaCapital"",
                                CAST(MAX(scc.""FECHA_ULT_PAGO"") AS date)                                      AS ""FechaUltimoPago"",
                                ROUND(MAX(scc.""VALOR_GESTION""), 2)::numeric(18,2)                            AS ""GastosCobranzas"",
                                ROUND(MAX(scc.""VALOR_MORA""), 2)::numeric(18,2)                               AS ""Interes"",
                                ROUND(MAX(t.valorliquidacion), 2)::numeric(18,2)                               AS ""MontoCobrar"",
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

            await _dataContext.Deudas
                              .Where(d => d.Empresa == "CRECOSCORP")
                              .ExecuteUpdateAsync(setters => setters
                              .SetProperty(d => d.EsActivo, false));

            List<Deudores> deudores = _dataContext.Deudores.ToList();
            var idsValidos = deudores
                .Where(d => d.IdDeudor != null)
                .Select(d => d.IdDeudor!)
                .ToHashSet();

            List<Usuario> usuarios = _dataContext.Usuarios
                .Where(x => x.Rol == "user")
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
                int i = 0;
                foreach (var d in lista)
                {
                    d.IdUsuario = usuarios[i].IdUsuario;
                    i = (i + 1) % usuarios.Count;
                }
            }
            void Asignar(List<Deuda> lista)
            {
                if (lista.Count == 0) return;
                var grupos = lista
                    .GroupBy(d => d.IdDeudor)
                    .Select(g => new { Items = g.ToList(), Total = g.Sum(x => x.DeudaCapital ?? 0m) })
                    .OrderByDescending(g => g.Total)
                    .ThenByDescending(g => g.Items.Count)
                    .ToList();

                var cargas = usuarios.ToDictionary(u => u.IdUsuario, _ => 0m);

                foreach (var g in grupos)
                {
                    var target = cargas.OrderBy(kv => kv.Value).First().Key;
                    foreach (var d in g.Items) d.IdUsuario = target;
                    cargas[target] += g.Total;
                }
            }

            Asignar(rowsMenos180);
            Asignar(rows181a360);
            Asignar(rowsMas360);

            var todas = rowsMenos180.Concat(rows181a360).Concat(rowsMas360).ToList();

            List<Deuda> deudas = _dataContext.Deudas.ToList();

            static DateTime? ToUtc(DateTime? dt)
    => dt.HasValue ? DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc) : (DateTime?)null;


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

            return Ok("Se insertó y actualizó correctamente");
        }

        [HttpPost("deudores/completo")]
        public async Task<IActionResult> ImportarDeudoresCompleto()
        {
            // var deudas = await svc.ImportarDeudasBasicoAsync();
            await svc.ImportarDeudoresCompletoAsync();
            await svc.ImportarTelefonosBasicoAsync();
            svc.GrabarTablas();

            return Ok(new { registrosAfectados = 2, telefonosAgregados = 2 });
        }



    }
}
