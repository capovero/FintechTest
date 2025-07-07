namespace DataFetcher.Application.DTOs;

public class FuturesApiResponseDto
{
    public string Symbol { get; set; } = string.Empty;
    public decimal Price { get; set; }
}