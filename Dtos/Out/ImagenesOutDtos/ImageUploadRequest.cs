namespace gestiones_backend.Dtos.Out.ImagenesOutDtos
{
    public class ImageUploadRequest
    {
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        public string Category { get; set; }
    }
}
