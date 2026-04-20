namespace StockAlert.Infrastructure.ExternalServices.Brapi.Models
{
    public class BrapiStockData
    {
        public string? Symbol { get; set; }
        public decimal RegularMarketPrice { get; set; }
        public long RegularMarketTime { get; set; }
    }
}
