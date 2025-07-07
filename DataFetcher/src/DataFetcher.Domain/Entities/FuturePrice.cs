namespace DataFetcher.Domain.Entities;

public class FuturePrice
{
    public string Symbol { get; set; } = string.Empty; 
    public string ContractCode { get; set; } = string.Empty; 
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}