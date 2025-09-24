using gestiones_backend.Dtos.Out;
using gestiones_backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IImageService imageService, ILogger<ImagesController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadModel uploadModel)
        {
            try
            {
                var result = await _imageService.SaveImageAsync(uploadModel);
                _logger.LogInformation("Imagen guardada: {FileName}", result.FileName);

                return Ok(new
                {
                    message = "Imagen subida exitosamente",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir imagen");
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
