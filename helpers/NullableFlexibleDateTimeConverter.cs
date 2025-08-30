using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace gestiones_backend.helpers
{
    public class NullableFlexibleDateTimeConverter : JsonConverter<DateTime?>
    {
        private static readonly string[] Formats = new[]
        {
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd",
        "dd/MM/yyyy",
        "d/M/yyyy",
        "M/d/yyyy",
        "MM/dd/yyyy",
        "dd/M/yyyy",
        "d/MM/yyyy",
        "dd/MM/yyyy HH:mm:ss",
        "M/d/yyyy HH:mm:ss",
        "yyyy/M/d",
        "yyyy/M/d HH:mm:ss"
    };

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;

            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (string.IsNullOrWhiteSpace(s)) return null;

                if (DateTime.TryParseExact(
                        s.Trim(),
                        Formats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces,
                        out var dt))
                    return dt;

                // último intento, parseo general
                if (DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dt))
                    return dt;

                throw new JsonException($"Fecha inválida: '{s}'.");
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                // soporta epoch miliseconds si fuera el caso
                var millis = reader.GetInt64();
                return DateTimeOffset.FromUnixTimeMilliseconds(millis).LocalDateTime;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            else
                writer.WriteNullValue();
        }
    }
}