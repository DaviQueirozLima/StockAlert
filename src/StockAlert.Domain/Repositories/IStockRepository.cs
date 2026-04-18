using StockAlert.Domain.Entities;

namespace StockAlert.Domain.Repositories
{
    public interface IStockRepository
    {
        Task<Stock?> GetBySymbolAsync(string symbol);
        Task AddAsync(Stock stock);
        Task UpdateAsync(Stock stock);
    }
}
