using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DataFetcher.Application.Interfaces;

namespace DataFetcher.API.Services
{
    public class FuturesFetcherService : IFuturesFetcherService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FuturesFetcherService> _logger;

        public FuturesFetcherService(HttpClient httpClient, ILogger<FuturesFetcherService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<decimal?> GetPriceAsync(string contractCode)
        {
            var url = $"https://fapi.binance.com/fapi/v1/ticker/price?symbol={contractCode}";
            string json = null;
            try
            {
                var resp = await _httpClient.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch price for {ContractCode}. Status: {StatusCode}", contractCode, resp.StatusCode);
                    return null;
                }

                json = await resp.Content.ReadAsStringAsync();
                _logger.LogDebug("Received response for {ContractCode}: {Json}", contractCode, json);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new DecimalStringConverter() }
                };
                var dto = JsonSerializer.Deserialize<BinanceTickerResponse>(json, options);

                if (dto?.Price == null)
                {
                    _logger.LogWarning("Invalid price received for {ContractCode}", contractCode);
                    return null;
                }

                _logger.LogInformation("Successfully fetched price for {ContractCode}: {Price}", contractCode, dto.Price);
                return dto.Price;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize price for {ContractCode}. JSON: {Json}", contractCode, json);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching price for {ContractCode}", contractCode);
                return null;
            }
        }
    }

    public class BinanceTickerResponse
    {
        public string Symbol { get; set; }

        [JsonPropertyName("price")]
        public decimal? Price { get; set; }

        public long Time { get; set; }
    }

    public class DecimalStringConverter : JsonConverter<decimal?>
    {
        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetDecimal(out var value))
                return value;

            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                    return null;

                if (decimal.TryParse(stringValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedValue))
                    return parsedValue;
            }

            throw new JsonException($"Cannot convert {reader.TokenType} to decimal. Value: {reader.GetString()}");
        }

        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            else
                writer.WriteNullValue();
        }
    }
}