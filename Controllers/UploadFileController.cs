using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Entity;
using gestiones_backend.helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

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
                    if (!actualizarDeudor.Any(d => d.IdDeudor == deudorExcel.Cedula))
                    {
                        deudorExistente.Nombre = deudorExcel.Nombre;
                        deudorExistente.Direccion = deudorExcel.Direccion;
                        deudorExistente.Telefono = deudorExcel.Telefono;
                        deudorExistente.Correo = deudorExcel.Correo;
                        deudorExistente.Descripcion = deudorExcel.Descripcion;

                        actualizarDeudor.Add(deudorExistente);
                    }
                }
                else
                {
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
        public IActionResult UploadFileDeudas([FromBody] List<UploadDeudasInDTO> deudasExcel, [FromQuery] bool desactivarDeudas = true )
        {
            List<string> empresasExcel = deudasExcel.Select(x => x.Empresa).Distinct().ToList();

            if (empresasExcel.Count > 1)
            {
                throw new Exception("Esta subiendo mas de una empresa: " +  String.Join(", ", empresasExcel) );
            }

            if (desactivarDeudas)
            {
                _context.Deudas
                   .Where(x => x.Empresa == empresasExcel[0] && x.EsActivo == true)
                   .ExecuteUpdate(setters => setters
                       .SetProperty(x => x.EsActivo, x => false)
                       .SetProperty(x => x.FechaRegistro, x => DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc))
                   );
            } 
           
            if (deudasExcel == null || deudasExcel.Count == 0)
                return BadRequest("No se recibieron datos");

            var deudoresValidos = new HashSet<string>(
                _context.Deudores.Select(d => d.IdDeudor).ToList()
            );

            var facturasExistentes = _context.Deudas
                .Where(d => deudasExcel.Select(x => x.NumeroFactura).Contains(d.NumeroFactura))
                .ToDictionary(d => d.NumeroFactura, d => d);

            List<Deuda> actualizarDeuda = new List<Deuda>();
            List<Deuda> grabarDeuda = new List<Deuda>();

            foreach (var deudaExcel in deudasExcel)
            {
                if (string.IsNullOrEmpty(deudaExcel.NumeroFactura))
                    continue;

                if (!deudoresValidos.Contains(deudaExcel.CedulaDeudor))
                    continue;

                if (facturasExistentes.TryGetValue(deudaExcel.NumeroFactura, out var deudaExistente))
                {
                    deudaExistente.IdDeudor = deudaExcel.CedulaDeudor;
                    deudaExistente.DeudaCapital = deudaExcel.DeudaCapital;
                    deudaExistente.Interes = deudaExcel.Interes;
                    deudaExistente.GastosCobranzas = deudaExcel.GastosCobranza;
                    deudaExistente.SaldoDeuda = deudaExcel.SaldoDeuda;
                    deudaExistente.Descuento = deudaExcel.Descuento;
                    deudaExistente.MontoCobrar = deudaExcel.MontoCobrar;
                    deudaExistente.FechaVenta = StringToDateOnly(deudaExcel.FechaVenta);
                    deudaExistente.FechaUltimoPago = StringToDateOnly(deudaExcel.FechaUltimoPago);
                    deudaExistente.Estado = deudaExcel.Estado;
                    deudaExistente.DiasMora = deudaExcel.DiasMora;
                    deudaExistente.NumeroFactura = deudaExcel.NumeroFactura;
                    deudaExistente.Clasificacion = deudaExcel.Calificacion;
                    deudaExistente.Creditos = deudaExcel.Creditos;
                    deudaExistente.NumeroCuotas = deudaExcel.NumeroCuotas;
                    deudaExistente.TipoDocumento = deudaExcel.TipoDeDocumento;
                    deudaExistente.ValorCuota = deudaExcel.ValorCuota;
                    deudaExistente.Tramo = deudaExcel.Tramo;
                    deudaExistente.UltimoPago = deudaExcel.UltimoPago;
                    deudaExistente.Empresa = deudaExcel.Empresa;
                    deudaExistente.ProductoDescripcion = deudaExcel.ProductoDescripcion;
                    deudaExistente.Agencia = deudaExcel.Agencia;
                    deudaExistente.Ciudad = deudaExcel.Ciudad;
                    deudaExistente.EsActivo = true;
                    deudaExistente.IdUsuario = deudaExcel.Usuario;
                    deudaExistente.FechaRegistro = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                    actualizarDeuda.Add(deudaExistente);
                }
                else
                {
                    var nuevaDeuda = new Deuda()
                    {
                        IdDeudor = deudaExcel.CedulaDeudor,
                        DeudaCapital = deudaExcel.DeudaCapital,
                        Interes = deudaExcel.Interes,
                        GastosCobranzas = deudaExcel.GastosCobranza,
                        SaldoDeuda = deudaExcel.SaldoDeuda,
                        Descuento = deudaExcel.Descuento,
                        MontoCobrar = deudaExcel.MontoCobrar,
                        FechaVenta = StringToDateOnly(deudaExcel.FechaVenta),
                        FechaUltimoPago = StringToDateOnly(deudaExcel.FechaUltimoPago),
                        Estado = deudaExcel.Estado,
                        DiasMora = deudaExcel.DiasMora,
                        NumeroFactura = deudaExcel.NumeroFactura,
                        Clasificacion = deudaExcel.Calificacion,
                        Creditos = deudaExcel.Creditos,
                        NumeroCuotas = deudaExcel.NumeroCuotas,
                        TipoDocumento = deudaExcel.TipoDeDocumento,
                        ValorCuota = deudaExcel.ValorCuota,
                        Tramo = deudaExcel.Tramo,
                        UltimoPago = deudaExcel.UltimoPago,
                        Empresa = deudaExcel.Empresa,
                        ProductoDescripcion = deudaExcel.ProductoDescripcion,
                        Agencia = deudaExcel.Agencia,
                        Ciudad = deudaExcel.Ciudad,
                        EsActivo = true,
                        IdUsuario = deudaExcel.Usuario,
                        FechaRegistro = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)

                    };
                    grabarDeuda.Add(nuevaDeuda);
                }
            }

            if (grabarDeuda.Count > 0)
                _context.Deudas.AddRange(grabarDeuda);

            if (actualizarDeuda.Count > 0)
                _context.Deudas.UpdateRange(actualizarDeuda);

            _context.SaveChanges();

            return Ok($"Procesadas {actualizarDeuda.Count} actualizaciones y {grabarDeuda.Count} inserciones");
        }

        private DateOnly? StringToDateOnly(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            string[] formatos =
            {
                "dd/MM/yyyy",
                "MM/dd/yyyy",
                "yyyy-MM-dd",
                "dd-MM-yyyy",
                "dd.MM.yyyy"
            };

            if (DateTime.TryParseExact(dateString, formatos,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var dateTime))
            {
                return DateOnly.FromDateTime(dateTime);
            }

            if (DateTime.TryParse(dateString, out var parsedDate))
            {
                return DateOnly.FromDateTime(parsedDate);
            }

            return null;
        }

    }
}
