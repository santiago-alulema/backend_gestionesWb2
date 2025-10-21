namespace gestiones_backend.Interfaces
{
    public interface ITrifocusExcelUploader
    {
        Task<string> GenerateAndUploadAsync(CancellationToken ct = default);
    }
}
