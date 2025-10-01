using gestiones_backend.Context;
using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.In.ImagenesInDtos;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Dtos.Out.ImagenesOutDtos;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IGestionarImagenes _imageService;
        private readonly DataContext _dataContext;
        private readonly ILogger<ImagesController> _logger;
        private readonly string _imagorBaseUrl;

        public ImagesController(IGestionarImagenes imageService, 
                                ILogger<ImagesController> logger,
                                DataContext dataContext,
                                IConfiguration configuration)
        {
            _imageService = imageService;
            _logger = logger;
            _dataContext = dataContext;
            _imagorBaseUrl = configuration["Imagor:BaseUrl"] ?? "http://localhost:8000";
        }

        [HttpPost("upload/{IdPago}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ImageResponse>> UploadImage( String IdPago, [FromForm] ImageUploadRequest request)
        {
            request.FileName = Guid.NewGuid().ToString("N").Substring(0,20);
            ImageResponse result = await _imageService.UploadImageAsync(request);
            ImagenesCobros nuevaImagen = new ImagenesCobros()
            {
                Id = Guid.NewGuid().ToString(),
                NombreArchivo = result.FileName,
                PagoId = Guid.Parse(IdPago),
                Tamanio = result.Size.ToString(),
                Url = result.Url,
                UrlRelativo = result.Url.Replace(_imagorBaseUrl,"")
            };
            _dataContext.ImagenesCobros.Add(nuevaImagen);
            _dataContext.SaveChanges();
            return Ok(result);
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteImage(string fileName)
        {
            try
            {
                var result = await _imageService.DeleteImageAsync(fileName);
                return result ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando imagen");
                return BadRequest(new { error = ex.Message });
            }
        }

        //[HttpGet("url/{fileName}")]
        //public ActionResult<string> GetImageUrl(string fileName, [FromQuery] int? width = null, [FromQuery] int? height = null)
        //{
        //    try
        //    {
        //        var url = _imageService.GetImageUrl(fileName, width, height);
        //        return Ok(new { url });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error generando URL");
        //        return BadRequest(new { error = ex.Message });
        //    }
        //}
    }
}
