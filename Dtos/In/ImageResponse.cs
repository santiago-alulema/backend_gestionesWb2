namespace gestiones_backend.Dtos.In
{
    public class ImageResponse
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public string? Category { get; set; }
    }
}
