using gestiones_backend.Context;
using gestiones_backend.Entity.temp_crecos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.Bulk;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrecosMetodosController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public CrecosMetodosController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost("grabar-campania")]
        public IActionResult GrabarCampaniaCrecos(List<TrifocusCrecosPartes> crecosPartes)
        {
            _dataContext.Database.ExecuteSqlRaw(@"
                TRUNCATE TABLE temp_crecos.trifocuscrecospartes 
                RESTART IDENTITY CASCADE;
            ");
            var bulk = new NpgsqlBulkUploader(_dataContext);
            bulk.Insert(crecosPartes);
            return Ok("Las liquidaciones de Crecos por parte se insertaron correctamente");
        }

    }
}
