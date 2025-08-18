using gestiones_backend.Dtos.Out;

namespace gestiones_backend.Interfaces
{
    public interface IReportesEmpresaService
    {
        Task<IEnumerable<ReporteEmpresaDto>> ObtenerReportePorEmpresaMesActual(string FechaInicio, string FechaFin);
    }
}
