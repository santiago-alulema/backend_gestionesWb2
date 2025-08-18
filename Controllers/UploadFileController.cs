using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Entity;
using gestiones_backend.helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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
                        deudorExistente.IdUsuario = deudorExcel.Usuario;

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
                List<String> dedudores = _context.Deudores.Select(x => x.IdDeudor).ToList();

                if (!dedudores.Contains(deudaExcel.CedulaDeudor))
                {
                    continue;
                }


                if (deudaExistente != null)
                {
                    deudaExistente.IdDeudor = deudaExcel.CedulaDeudor;
                    deudaExistente.DeudaCapital = deudaExcel.DeudaCapital;
                    deudaExistente.Interes = deudaExcel.Interes;
                    deudaExistente.GastosCobranzas = deudaExcel.GastosCobranza;
                    deudaExistente.SaldoDeuda = deudaExcel.SaldoDeuda;
                    deudaExistente.Descuento = (int)(Decimal.Parse(deudaExcel.Descuento.Replace("%", "")) * 100);
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
                        Descuento = (int)(Decimal.Parse(deudaExcel.Descuento.Replace("%", "")) * 100),
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
                        Empresa = deudaExcel.Empresa
                    };
                    grabarDeuda.Add(nuevaDeuda);
                }
            }
            if (grabarDeuda.Count > 0)
                _context.Deudas.AddRange(grabarDeuda);
            if(actualizarDeuda.Count > 0)
                _context.Deudas.UpdateRange(actualizarDeuda);
            _context.SaveChanges();
            return Ok("Deudas procesadas exitosamente (actualizaciones e inserciones)");
           
        }
        public static DateOnly? StringToDateOnly(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            if (DateTime.TryParse(dateString, out DateTime dateTime))
                return DateOnly.FromDateTime(dateTime);

            // Intenta con formatos específicos
            if (DateOnly.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
                return date;

            return null;
        }
    }
}
