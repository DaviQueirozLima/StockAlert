namespace StockAlert.Communication.Responses.Stock
{
    public class StockResponse
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? Source { get; set; }
    }
}
