using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;
using gestiones_backend.Interfaces;

namespace gestiones_backend.Services
{
    public class ImageService : IImageService
    {
        private readonly string _imagesPath;
        private readonly IWebHostEnvironment _environment;

        public ImageService(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _imagesPath = "http://localhost:9000/app/images" ?? "/app/images";

            // Crear directorio si no existe
            if (!Directory.Exists(_imagesPath))
            {
                Directory.CreateDirectory(_imagesPath);
            }
        }

        public async Task<ImageResponse> SaveImageAsync(ImageUploadModel uploadModel)
        {
            if (uploadModel.ImageFile == null || uploadModel.ImageFile.Length == 0)
            {
                throw new ArgumentException("No se proporcionó una imagen válida");
            }

            // Validar extensión
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(uploadModel.ImageFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Formato de archivo no permitido");
            }

            // Generar nombre único
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_imagesPath, fileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadModel.ImageFile.CopyToAsync(stream);
            }

            return new ImageResponse
            {
                FileName = fileName,
                FilePath = filePath,
                FileSize = uploadModel.ImageFile.Length,
                UploadDate = DateTime.UtcNow,
                Category = uploadModel.Category
            };
        }

        public async Task<byte[]> GetImageAsync(string fileName)
        {
            var filePath = Path.Combine(_imagesPath, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Imagen no encontrada");
            }

            return await File.ReadAllBytesAsync(filePath);
        }

        public Task<bool> DeleteImageAsync(string fileName)
        {
            var filePath = Path.Combine(_imagesPath, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<List<string>> GetAllImagesAsync()
        {
            var files = Directory.GetFiles(_imagesPath)
                .Select(Path.GetFileName)
                .Where(name => name != null)
                .ToList();

            return Task.FromResult(files!);
        }
    }
}