using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql.Bulk;

namespace gestiones_backend.Services
{
    
    public class MetodosCrecosServices : IMetodosCrecos
    {
        private readonly DataContext _dataContext;
        public MetodosCrecosServices(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public string AsignacionAutomaticaDeudasCrecos()
        {
            List<Usuario> usuariosActivos = _dataContext.Usuarios.Where(x => x.EstaActivo && x.Rol == "user").ToList();
            var deudasCrecosActivas = _dataContext.Deudas.Where(x => x.EsActivo == true && 
            x.Empresa.Contains("CRECO") && 
            x.IdUsuario == null).ToList();

            int cantidadUsuariosActivos = usuariosActivos.Count();
            int contador = 0;
            foreach (var item in deudasCrecosActivas)
            {
                item.IdUsuario = usuariosActivos[contador].IdUsuario;
                contador++;
                if (contador == cantidadUsuariosActivos) contador = 0;
            }
            _dataContext.SaveChanges();
            return "Ok";
        }

        public string AsignardeudaNullIdUsuario(string IdUsuario)
        {
            var deudasCrecosActivasUsuario = _dataContext.Deudas.Where(x => x.EsActivo == true &&
            x.Empresa.Contains("CRECO") &&
            x.IdUsuario == IdUsuario).ToList();
            foreach (var item in deudasCrecosActivasUsuario)
            {
                item.IdUsuario = null;
            }
            _dataContext.SaveChanges();
            return "Ok";
        }

        public async Task<bool> SubirDeudasCrecosMasivomanual(List<DeudasCrecosMasivoInDto> deudas, CancellationToken ct)
        {
            if (deudas.Count == 0){
                throw new Exception("No ingreso ninguna data.");
            }

            _dataContext.Deudas
                          .Where(d => d.Empresa == "CRECOSCORP")
                          .ExecuteUpdate(setters => setters
                          .SetProperty(d => d.EsActivo, false));

            List<Deuda> deudasBD = await _dataContext.Deudas.Where(x => x.Empresa.Contains("CRECOSCORP")).ToListAsync(ct);
            List<Deuda> deudasUpdate = new();
            foreach (var item in deudas)
            {
                Deuda deuda = deudasBD.FirstOrDefault(x => x.IdDeudor == item.IdDeudor && x.Empresa.Contains("CRECOSCORP"));

                if( deuda != null)
                {
                    deuda.SaldoDeuda = item.SaldoDeuda;
                    deuda.DiasMora = item.DiasMora;
                    deuda.Tramo = item.Tramo;
                    deuda.UltimoPago = item.UltimoPago;
                    deuda.Empresa = "CRECOSCORP";
                    deuda.Clasificacion = item.Clasificacion;
                    deuda.Creditos = item.Creditos;
                    deuda.Descuento = item.Descuento;
                    deuda.FechaUltimoPago = item.FechaUltimoPago.HasValue
                                            ? DateOnly.FromDateTime(item.FechaUltimoPago.Value)
                                            : null;
                    deuda.MontoCobrar = item.MontoCobrar;
                    deuda.TipoDocumento = item.TipoDocumento;
                    deuda.Agencia = item.Agencia;
                    deuda.Ciudad = item.Ciudad;
                    deuda.EsActivo = true;
                    deuda.CodigoEmpresa = "000001";
                    deuda.CodigoOperacion = item.CodigoOperacion;
                    deuda.MontoCobrarPartes = item.MontoCobrarPartes;
                    deuda.MontoPonteAlDia = item.MontoPonteAlDia;
                    deuda.IdUsuario = item.Gestor ?? "";
                    deudasUpdate.Add(deuda);
                }
            }

            var bulk = new NpgsqlBulkUploader(_dataContext);
            bulk.Update(deudasUpdate);
            return true;
        }
    }
}
