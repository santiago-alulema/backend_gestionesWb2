using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.In.ImagenesInDtos;
using gestiones_backend.Dtos.Out.ImagenesOutDtos;

namespace gestiones_backend.Interfaces
{
    public interface IGestionarImagenes
    {
        Task<ImageResponse> UploadImageAsync(ImageUploadRequest request);
        Task<bool> DeleteImageAsync(string fileName);
        string GenerateSignedUrl(string imagePath, int? width = null, int? height = null);
        string GetImageUrl(string fileName, int? width = null, int? height = null);
    }
}
