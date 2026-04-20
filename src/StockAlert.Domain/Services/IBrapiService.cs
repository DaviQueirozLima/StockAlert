using StockAlert.Domain.Services.Dtos;

namespace StockAlert.Domain.Services
{
    public interface IBrapiService
    {
        Task<StockQuoteDto?> GetStockQuoteAsync(string stockSymbol);
    }
}
