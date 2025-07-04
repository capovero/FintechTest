namespace SpreadCalculator.Domain.Entities;

public class FuturePrice
{
    public int Id { get; set; }
    public string Symbol { get; set; } = default!;         
    public string ContractCode { get; set; } = default!;   
    public DateTime Timestamp { get; set; }
    public decimal Price { get; set; }   
}