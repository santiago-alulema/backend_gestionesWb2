namespace gestiones_backend.Dtos.In.ImagenesInDtos
{
    public class ImageResponse
    {
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
