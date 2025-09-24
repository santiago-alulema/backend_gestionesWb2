using gestiones_backend.Dtos.In;
using gestiones_backend.Dtos.Out;

namespace gestiones_backend.Interfaces
{
    public interface IImageService
    {
        Task<ImageResponse> SaveImageAsync(ImageUploadModel uploadModel);
        Task<byte[]> GetImageAsync(string fileName);
        Task<bool> DeleteImageAsync(string fileName);
        Task<List<string>> GetAllImagesAsync();
    }
}
