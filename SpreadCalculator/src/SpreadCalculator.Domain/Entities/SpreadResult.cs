namespace SpreadCalculator.Domain.Entities;

public class SpreadResult
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal NearPrice { get; set; }
    public decimal FarPrice { get; set; }
    public decimal Spread => FarPrice - NearPrice;
}