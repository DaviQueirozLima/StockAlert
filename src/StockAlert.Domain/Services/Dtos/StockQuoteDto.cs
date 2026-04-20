namespace StockAlert.Domain.Services.Dtos
{
    public class StockQuoteDto
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime LastRefresh { get; set; }
    }
}
