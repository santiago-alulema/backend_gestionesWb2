using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Entity;
using gestiones_backend.helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly DataContext _context;

        public UploadFileController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("upload-excel-deudores")]
        public IActionResult UploadFileDeudores([FromBody] List<UploadDeudoresInDTO> deudoresExcel)
        {
            if (deudoresExcel == null || !deudoresExcel.Any())
                return BadRequest("No se recibieron datos.");

            var cedulasExcel = deudoresExcel.Select(d => d.Cedula).ToList();
            var deudoresExistentes = _context.Deudores
                                           .Where(d => cedulasExcel.Contains(d.IdDeudor))
                                           .ToDictionary(d => d.IdDeudor);

            var actualizarDeudor = new List<Deudores>();
            var grabarDeudor = new List<Deudores>();

            foreach (var deudorExcel in deudoresExcel)
            {
                if (deudoresExistentes.TryGetValue(deudorExcel.Cedula, out var deudorExistente))
                {
                    // Actualizar solo si no está ya en la lista de actualización
                    if (!actualizarDeudor.Any(d => d.IdDeudor == deudorExcel.Cedula))
                    {
                        deudorExistente.Nombre = deudorExcel.Nombre;
                        deudorExistente.Direccion = deudorExcel.Direccion;
                        deudorExistente.Telefono = deudorExcel.Telefono;
                        deudorExistente.Correo = deudorExcel.Correo;
                        deudorExistente.Descripcion = deudorExcel.Descripcion;
                        deudorExistente.IdUsuario = deudorExcel.Usuario;

                        actualizarDeudor.Add(deudorExistente);
                    }
                }
                else
                {
                    // Agregar solo si no está ya en la lista de grabación
                    if (!grabarDeudor.Any(d => d.IdDeudor == deudorExcel.Cedula))
                    {
                        grabarDeudor.Add(new Deudores
                        {
                            IdDeudor = deudorExcel.Cedula,
                            Nombre = deudorExcel.Nombre,
                            Direccion = deudorExcel.Direccion,
                            Telefono = deudorExcel.Telefono,
                            Correo = deudorExcel.Correo,
                            Descripcion = deudorExcel.Descripcion,
                            IdUsuario = deudorExcel.Usuario
                        });
                    }
                }
            }

            if (grabarDeudor.Count > 0)
                _context.Deudores.AddRange(grabarDeudor);

            if (actualizarDeudor.Count > 0)
                _context.Deudores.UpdateRange(actualizarDeudor);

            _context.SaveChanges();

            return Ok($"Datos procesados exitosamente: {grabarDeudor.Count} insertados, {actualizarDeudor.Count} actualizados.");
        }

        [HttpPost("upload-excel-deudas")]
        public IActionResult uploadFileDeudas([FromBody] List<UploadDeudasInDTO> deudasExcel)
        {
            List<Deuda> actualizarDeuda = [];
            List<Deuda> grabarDeuda = [];

            foreach (var deudaExcel in deudasExcel)
            {
                if (string.IsNullOrEmpty(deudaExcel.NumeroFactura))
                {
                    continue; 
                }

                var deudaExistente = _context.Deudas
                    .FirstOrDefault(d => d.NumeroFactura == deudaExcel.NumeroFactura);

                if (deudaExistente != null)
                {
                    deudaExistente.IdDeudor = deudaExcel.CedulaDeudor;
                    deudaExistente.MontoOriginal = deudaExcel.MontoOriginal;
                    deudaExistente.SaldoActual = deudaExcel.SaldoActual;
                    deudaExistente.FechaVencimiento = DateOnly.Parse(deudaExcel.FechaVencimiento);

                    if (!string.IsNullOrEmpty(deudaExcel.FechaAsignacion))
                        deudaExistente.FechaAsignacion = DateOnly.Parse(deudaExcel.FechaAsignacion);

                    deudaExistente.Estado = deudaExcel.Estado;
                    deudaExistente.Descripcion = deudaExcel.Descripcion;
                    deudaExistente.NumeroAutorizacion = deudaExcel.NumeroAutorizacion;
                    deudaExistente.TotalFactura = deudaExcel.TotalFactura;
                    deudaExistente.SaldoDeuda = deudaExcel.SaldoDeuda;
                    deudaExistente.NumeroCuotas = deudaExcel.NumeroCuotas;
                    deudaExistente.CuotaActual = deudaExcel.CuotaActual;
                    deudaExistente.ValorCuota = deudaExcel.ValorCuota;
                    deudaExistente.Tramo = deudaExcel.Tramo;
                    deudaExistente.UltimoPago = deudaExcel.UltimoPago;
                    deudaExistente.Empresa = deudaExcel.Empresa;
                    actualizarDeuda.Add(deudaExistente);
                }
                else
                {
                    var nuevaDeuda = new Deuda()
                    {
                        IdDeudor = deudaExcel.CedulaDeudor,
                        MontoOriginal = deudaExcel.MontoOriginal,
                        SaldoActual = deudaExcel.SaldoActual,
                        FechaVencimiento = DateOnly.Parse(deudaExcel.FechaVencimiento),
                        FechaAsignacion = !string.IsNullOrEmpty(deudaExcel.FechaAsignacion) ?
                            DateOnly.Parse(deudaExcel.FechaAsignacion) : null,
                        Estado = deudaExcel.Estado,
                        Descripcion = deudaExcel.Descripcion,
                        NumeroFactura = deudaExcel.NumeroFactura,
                        NumeroAutorizacion = deudaExcel.NumeroAutorizacion,
                        TotalFactura = deudaExcel.TotalFactura,
                        SaldoDeuda = deudaExcel.SaldoDeuda,
                        NumeroCuotas = deudaExcel.NumeroCuotas,
                        CuotaActual = deudaExcel.CuotaActual,
                        ValorCuota = deudaExcel.ValorCuota,
                        Tramo = deudaExcel.Tramo,
                        UltimoPago = deudaExcel.UltimoPago,
                        Empresa = deudaExcel.Empresa
                    };
                    grabarDeuda.Add(nuevaDeuda);
                }
            }
            _context.Deudas.AddRange(grabarDeuda);
            _context.Deudas.UpdateRange(actualizarDeuda);
            _context.SaveChanges();
            return Ok("Deudas procesadas exitosamente (actualizaciones e inserciones)");
           
        }

        //[HttpPost("upload-excel-deudas")]
        //public IActionResult uploadFileDeudas([FromBody] List<UploadDeudasInDTO> deudasExcel)
        //{
        //    List<Deuda> deudas = new();
        //    for (int i = 0; i < deudasExcel.Count; i++)
        //    {
        //        deudas.Add(new Deuda()
        //        {
        //            IdDeudor = deudasExcel[i].CedulaDeudor,
        //            MontoOriginal = deudasExcel[i].MontoOriginal,
        //            SaldoActual = deudasExcel[i].SaldoActual,
        //            FechaVencimiento = DateOnly.Parse( deudasExcel[i].FechaVencimiento),
        //            FechaAsignacion = DateOnly.Parse( deudasExcel[i].FechaAsignacion),
        //            Estado = deudasExcel[i].Estado,
        //            Descripcion = deudasExcel[i].Descripcion,
        //            NumeroFactura = deudasExcel[i].NumeroFactua,
        //            NumeroAutorizacion = deudasExcel[i].NumeroAutorizacion,
        //            TotalFactura = deudasExcel[i].TotalFactura,

        //            SaldoDeuda = deudasExcel[i].SaldoDeuda,
        //            NumeroCuotas = deudasExcel[i].NumeroCuotas,
        //            CuotaActual = deudasExcel[i].CuotaActual,
        //            ValorCuota = deudasExcel[i].ValorCuota,
        //            Tramo = deudasExcel[i].Tramo,
        //            UltimoPago = deudasExcel[i].UltimoPago,
        //            Empresa = deudasExcel[i].Empresa,

        //        });
        //    }
        //    _context.Deudas.AddRange(deudas);
        //    _context.SaveChanges();
        //    return Ok("Se grabo exitosamente");
        //}
    }
}
