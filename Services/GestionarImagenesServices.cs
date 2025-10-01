using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.In.ImagenesInDtos;
using gestiones_backend.Dtos.Out.ImagenesOutDtos;
using gestiones_backend.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace gestiones_backend.Services
{
    public class GestionarImagenesServices : IGestionarImagenes
    {

        private readonly string _imagorBaseUrl;
        private readonly string _secretKey;
        private readonly string _uploadPath;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpClientFactory _httpClientFactory;

        public GestionarImagenesServices(IConfiguration configuration, IWebHostEnvironment environment, IHttpClientFactory httpClientFactory)
        {
            _imagorBaseUrl = configuration["Imagor:BaseUrl"] ?? "http://localhost:8000";
            _secretKey = configuration["Imagor:SecretKey"] ?? "reemplaza_con_llave_larga_1";
            _uploadPath = configuration["Imagor:UploadPath"] ?? "/images";
            _environment = environment;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ImageResponse> UploadImageAsync(ImageUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                throw new ArgumentException("Archivo inválido");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException("Tipo de archivo no permitido");

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }


            var fileName = string.IsNullOrEmpty(request.FileName)
                ? $"{Guid.NewGuid()}{fileExtension}"
                : $"{Path.GetFileNameWithoutExtension(request.FileName)}{fileExtension}";

            var physicalPath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            var originalUrl = GetImageUrl(fileName);
            var thumbnailUrl = GenerateSignedUrl(fileName, 300, 300);

            return new ImageResponse
            {
                Url = originalUrl,
                ThumbnailUrl = thumbnailUrl,
                FileName = fileName,
                Size = request.File.Length,
                UploadedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> DeleteImageAsync(string fileName)
        {
            var physicalPath = Path.Combine(_uploadPath, fileName);

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
                return true;
            }
            return false;
        }

        public string GetImageUrl(string fileName, int? width = null, int? height = null)
        {
            if (width == null && height == null)
                return $"{_imagorBaseUrl}/unsafe/{fileName}";

            var dimensions = "";
            if (width.HasValue && height.HasValue)
                dimensions = $"{width}x{height}";
            else if (width.HasValue)
                dimensions = $"{width}x0";
            else if (height.HasValue)
                dimensions = $"0x{height}";

            return GenerateSignedUrl($"{dimensions}/{fileName}");
        }

        public string GenerateSignedUrl(string imagePath, int? width = null, int? height = null)
        {
            string path = imagePath.TrimStart('/');

            string ops = null;
            if (width.HasValue || height.HasValue)
            {
                var w = width?.ToString() ?? "0";
                var h = height?.ToString() ?? "0";
                ops = $"{w}x{h}";
            }

            if (ops is not null) path = $"{ops}/{path}";

            if (_environment.IsDevelopment())
                return $"{_imagorBaseUrl}/unsafe/{path}";

            var signature = GenerateSignature(path); 
            return $"{_imagorBaseUrl}/{signature}/{path}";
        }


        private string GenerateSignature(string path)
        {
            using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(_secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(path));

            return Convert.ToBase64String(hash)
                .Replace('+', '-')
                .Replace('/', '_');
        }

    }
}
