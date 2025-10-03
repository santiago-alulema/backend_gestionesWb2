namespace gestiones_backend.Dtos.In.EmailInDtos
{
    public class EmailRequestDto
    {
        public string To { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string HtmlBody { get; set; } = default!;
        public string? PlainTextBody { get; set; }
    }
}
