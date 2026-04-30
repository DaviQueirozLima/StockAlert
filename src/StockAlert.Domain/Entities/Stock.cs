namespace StockAlert.Domain.Entities
{
    public class Stock
    {
        public string Symbol { get; set; } = default!;
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public decimal CurrentPrice { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Fonte do preço (ex: "YahooFinance", "AlphaVantage, Brapi")
        public string? Source { get; set; }

    }
}
