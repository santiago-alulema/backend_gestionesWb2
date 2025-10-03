namespace gestiones_backend.Interfaces
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(
                            string to,
                            string subject,
                            string htmlBody,
                            string? plainTextBody = null,
                            IEnumerable<string>? cc = null,
                            IEnumerable<string>? bcc = null,
                            IEnumerable<(string FileName, byte[] Content, string? ContentType)>? attachments = null,
                            string configKey = "default",
                            CancellationToken ct = default);

        Task<bool> VerifySenderAsync(string configKey = "default", CancellationToken ct = default);

        Task<bool?> TryVerifyMailboxAsync(string email, string configKey = "default", CancellationToken ct = default);
    }
}
