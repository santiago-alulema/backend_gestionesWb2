using gestiones_backend.Context;
using gestiones_backend.Entity;
using gestiones_backend.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System;

namespace gestiones_backend.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly DataContext _db;

        public EmailSenderService(DataContext db)
        {
            _db = db;
        }

        private async Task<EmailSmtpConfig> GetConfigAsync(string configKey, CancellationToken ct)
        {
            var cfg = await _db.EmailSmtpConfigs
                .AsNoTracking()
                .Where(c => c.Key == configKey && c.IsActive)
                .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
                .FirstOrDefaultAsync(ct);

            if (cfg is null)
                throw new InvalidOperationException($"No existe configuración SMTP activa con Key='{configKey}'.");

            return cfg;
        }

        private static SecureSocketOptions GetSocketOptions(EmailSmtpConfig cfg)
        {
            if (cfg.UseSsl) return SecureSocketOptions.SslOnConnect;   
            if (cfg.UseStartTls) return SecureSocketOptions.StartTls;  
            return SecureSocketOptions.Auto;                            
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string htmlBody,
            string? plainTextBody = null,
            IEnumerable<string>? cc = null,
            IEnumerable<string>? bcc = null,
            IEnumerable<(string FileName, byte[] Content, string? ContentType)>? attachments = null,
            string configKey = "default",
            CancellationToken ct = default)
        {
            var cfg = await GetConfigAsync(configKey, ct);

            var message = new MimeMessage();

            message.From.Add(MailboxAddress.Parse(cfg.Username));

            message.To.Add(MailboxAddress.Parse(to));

            if (cc != null)
                foreach (var c in cc)
                    message.Cc.Add(MailboxAddress.Parse(c));

            if (bcc != null)
                foreach (var c in bcc)
                    message.Bcc.Add(MailboxAddress.Parse(c));

            message.Subject = subject;

            var builder = new BodyBuilder
            {
                TextBody = plainTextBody ?? TextFormatFromHtml(htmlBody),
                HtmlBody = htmlBody
            };

            if (attachments != null)
            {
                foreach (var (fileName, content, contentType) in attachments)
                {
                    if (!string.IsNullOrWhiteSpace(contentType))
                        builder.Attachments.Add(fileName, content, ContentType.Parse(contentType!));
                    else
                        builder.Attachments.Add(fileName, content);
                }
            }

            message.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            smtp.Timeout = cfg.TimeoutSeconds * 1000;

            var options = GetSocketOptions(cfg);
            await smtp.ConnectAsync(cfg.SmtpHost, cfg.SmtpPort, options, ct);

            // Autenticación
            await smtp.AuthenticateAsync(cfg.Username, cfg.Password, ct);

            await smtp.SendAsync(message, ct);
            await smtp.DisconnectAsync(true, ct);
        }

        public async Task<bool> VerifySenderAsync(string configKey = "default", CancellationToken ct = default)
        {
            var cfg = await GetConfigAsync(configKey, ct);

            try
            {
                using var smtp = new SmtpClient();
                smtp.Timeout = cfg.TimeoutSeconds * 1000;

                var options = GetSocketOptions(cfg);
                await smtp.ConnectAsync(cfg.SmtpHost, cfg.SmtpPort, options, ct);

                await smtp.AuthenticateAsync(cfg.Username, cfg.Password, ct);

                await smtp.NoOpAsync(ct);
                await smtp.DisconnectAsync(true, ct);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool?> TryVerifyMailboxAsync(string email, string configKey = "default", CancellationToken ct = default)
        {
            // En MailKit 4.14 no hay Verify y no es fiable implementar RCPT probing.
            return Task.FromResult<bool?>(null);
        }

        private static string TextFormatFromHtml(string html)
        {
            var plain = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
            return System.Net.WebUtility.HtmlDecode(plain).Trim();
        }
    }
}