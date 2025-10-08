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
                                             gen_random_uuid()  AS ""IdDeuda"",
'NA'                                         AS ""Estado"",
                                              ca.""CNUMEROIDENTIFICACION""                               AS ""IdDeudor"",
                                              CAST(occ.""FECHA_FACTURA"" AS date)                        AS ""FechaVenta"",
                                              occ.""NUM_FACTURA""                                        AS ""NumeroFactura"",
                                              ROUND(scc.""VALOR_MORA"" + scc.""VALOR_GESTION"" + scc.""VALOR_DEUDA"" , 2) AS ""SaldoDeuda"",
                                              occ.""NUM_CUOTAS""                                         AS ""NumeroCuotas"",
                                              scc.""DIAS_VENCIDOS""                                      AS ""DiasMora"",
                                              0                                                          AS ""ValorCuota"",
                                              scc.""CDESCRIPCIONTRAMO""                                  AS ""Tramo"",
                                              NULL                                                       AS ""UltimoPago"",
                                              'CRECOS'                                                   AS ""Empresa"",
                                              NULL                                                       AS ""Clasificacion"",
                                              1                                                          AS ""Creditos"",
                                              ROUND((t.valorliquidacion / NULLIF(scc.""VALOR_DEUDA"",0))*100, 0) AS ""Descuento"",
                                              ROUND(scc.""VALOR_DEUDA"" , 2)                             As DeudaCapital,
                                              CAST(scc.""FECHA_ULT_PAGO"" AS date) AS ""FechaUltimoPago"",
                                              ROUND(scc.""VALOR_GESTION"", 2)                            AS ""GastosCobranzas"",
                                              ROUND(scc.""VALOR_MORA"", 2)                               AS ""Interes"",
                                              ROUND(t.valorliquidacion, 2)                               AS ""MontoCobrar"",
                                              dcc.""DESCRIP_TIPO_IDENTIF""                               AS ""TipoDocumento"",
                                              'Creditos Economicos'                                      AS ""Agencia"",
                                              ca.""CANTON""                                              AS ""Ciudad"",
                                              (
                                                SELECT string_agg(aoc.""DESC_PRODUCTO"", ' || ')
                                                FROM temp_crecos.""ArticuloOperacionCrecos"" aoc
                                                WHERE aoc.""COD_OPERACION"" = occ.""ICODIGOOPERACION""
                                              )                                                          AS ""ProductoDescripcion"",
                                              TRUE                                                       AS ""EsActivo"",
                                              NULL                                                       AS ""IdUsuario"",
                                              ca.""COD_EMPRESA""                                         AS ""CodigoEmpresa"",
                                              now()                                                      AS ""FechaRegistro"",
                                             ROUND(t.valorliquidacionparte, 2)                           AS ""MontoCobrarPartes"",
                                              occ.""ICODIGOOPERACION""                                   AS ""CodigoOperacion""
                                            FROM temp_crecos.""CarteraAsignadaCrecos"" ca
                                            LEFT JOIN temp_crecos.""DatosClienteCrecos"" dcc 
                                              ON dcc.""ICODIGOCLIENTE"" = ca.""CODIGOCLIENTE""
                                            LEFT JOIN temp_crecos.""OperacionesClientesCrecos"" occ 
                                              ON occ.""N_IDENTIFICACION"" = ca.""CNUMEROIDENTIFICACION""
                                            LEFT JOIN temp_crecos.trifocuscrecospartes t 
                                              ON t.codoperacion = occ.""ICODIGOOPERACION""
                                            LEFT JOIN temp_crecos.""SaldoClienteCrecos"" scc 
                                              ON ca.""CODIGOCLIENTE"" = scc.""CODIGOCLIENTE"";";


            List<Deuda> rows = _dataContext.Deudas
                                        .FromSqlRaw(SqlDeudasCrecos)
                                        .AsNoTracking()
                                        .ToList();

            List<Deudores> deudores = _dataContext.Deudores.ToList();
            var idsValidos = deudores
                .Where(d => d.IdDeudor != null)
                .Select(d => d.IdDeudor!)
                .ToHashSet();

            List<Usuario> usuarios = _dataContext.Usuarios
                .Where(x => x.Rol == "user")
                .ToList();

            // Filtrar solo los que existen en Deudores
            rows = rows
                .Where(r => r.IdDeudor != null && idsValidos.Contains(r.IdDeudor!))
                .ToList();

            // Clasificación por días de mora
            List<Deuda> rowsMenos180 = rows.Where(x => x.DiasMora < 180).ToList();
            List<Deuda> rows181a360 = rows.Where(x => x.DiasMora >= 180 && x.DiasMora <= 360).ToList();
            List<Deuda> rowsMas360 = rows.Where(x => x.DiasMora > 360).ToList();

            // Asignar usuario por round-robin
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

            AsignarUsuario(rowsMenos180);
            AsignarUsuario(rows181a360);
            AsignarUsuario(rowsMas360);

            // Unir todas las deudas
            var todas = rowsMenos180.Concat(rows181a360).Concat(rowsMas360).ToList();

            // ---- UPSET SIMPLE POR NumeroFactura ----
            foreach (var deuda in todas)
            {
                var existente = _dataContext.Deudas
                    .FirstOrDefault(d => d.NumeroFactura == deuda.NumeroFactura);

                if (existente != null)
                {
                    // Actualizar campos existentes
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

                    existente.Empresa = deuda.Empresa;
                    existente.Tramo = deuda.Tramo;
                    existente.UltimoPago = deuda.UltimoPago;
                    existente.MontoCobrarPartes = deuda.MontoCobrarPartes;

                    existente.IdUsuario = deuda.IdUsuario;

                    _dataContext.Deudas.Update(existente);
                }
                else
                {
                    // Insertar nuevo
                    if (deuda.IdDeuda == Guid.Empty)
                        deuda.IdDeuda = Guid.NewGuid();

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

            svc.GrabarTablas();
            var telefonos = await svc.ImportarDeudoresCompletoAsync();

            return Ok(new { registrosAfectados = 2, telefonosAgregados = 2 });
        }
    }
}
