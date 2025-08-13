namespace gestiones_backend.Dtos.Out
{
    public class ReporteEmpresaDto
    {
        public string Empresa { get; set; }
        public int CantidadGestiones { get; set; }
        public int CantidadCompromisosPago { get; set; }
        public int CantidadPagos { get; set; }
        public decimal ValorTotalPagos { get; set; }
        public decimal ValorTotalCompromisos { get; set; }
    }
}
