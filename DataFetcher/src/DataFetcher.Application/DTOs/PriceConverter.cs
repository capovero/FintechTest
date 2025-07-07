using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataFetcher.Application.DTOs
{
    public class PriceConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String &&
                decimal.TryParse(reader.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var val))
                return val;

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetDecimal(out var num))
                return num;

            return 0;
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value);
    }
}