namespace gestiones_backend.Dtos.In
{
    public class UploadDeudasInDTO
    {
        public string? CedulaDeudor { get; set; }

        public decimal MontoOriginal { get; set; }

        public decimal SaldoActual { get; set; }

        public String FechaVencimiento { get; set; }

        public String? FechaAsignacion { get; set; }

        public string? Estado { get; set; }

        public string? Descripcion { get; set; }
        public string? NumeroFactura { get; set; }
        public string? NumeroAutorizacion { get; set; }
        public Decimal? TotalFactura { get; set; }


        public Decimal? SaldoDeuda { get; set; }
        public int? NumeroCuotas { get; set; }
        public int? CuotaActual { get; set; }
        public Decimal? ValorCuota { get; set; }
        public string? Tramo { get; set; }
        public Decimal? UltimoPago { get; set; }
        public string? Empresa { get; set; }



    }
}
