namespace StockAlert.Domain.Entities
{
    public class Stock
    {
        // Você pode usar Symbol como chave primária se preferir
        public string Symbol { get; set; } = default!;

        public decimal CurrentPrice { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Fonte do preço (ex: "YahooFinance", "AlphaVantage")
        public string? Source { get; set; }

    }
}
