using System.Text.Json.Serialization;

namespace DataFetcher.Application.DTOs
{
    public class BinanceTickerResponse
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = null!;

        [JsonPropertyName("price")]
        [JsonConverter(typeof(PriceConverter))]
        public decimal Price { get; set; }
    }
}