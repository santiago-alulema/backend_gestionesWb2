using gestiones_backend.Dtos.In.EmailInDtos;
using gestiones_backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gestiones_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSenderService _emailSender;

        public EmailController(IEmailSenderService emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequestDto request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.To) ||
                string.IsNullOrWhiteSpace(request.Subject) ||
                string.IsNullOrWhiteSpace(request.HtmlBody))
            {
                return BadRequest(new { success = false, message = "Faltan campos obligatorios: To, Subject o HtmlBody." });
            }

            try
            {
                await _emailSender.SendEmailAsync(
                    to: request.To,
                    subject: request.Subject,
                    htmlBody: request.HtmlBody,
                    plainTextBody: request.PlainTextBody,
                    configKey: "default",
                    ct: ct
                );

                return Ok(new { success = true, message = $"Correo enviado a {request.To}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyConfig(CancellationToken ct)
        {
            var result = await _emailSender.VerifySenderAsync("default", ct);
            return Ok(new { success = result });
        }
    }
}
