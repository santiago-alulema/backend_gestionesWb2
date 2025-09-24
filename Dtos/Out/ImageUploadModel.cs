namespace gestiones_backend.Dtos.Out
{
    public class ImageUploadModel
    {
        public IFormFile ImageFile { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
    }
}
