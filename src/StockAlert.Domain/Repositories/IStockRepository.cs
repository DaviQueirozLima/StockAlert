using StockAlert.Domain.Entities;

namespace StockAlert.Domain.Repositories
{
    public interface IStockRepository
    {
        Task<Stock?> GetBySymbolAndUserIdAsync(string symbol, Guid userId);
        Task AddAsync(Stock stock);
        Task UpdateAsync(Stock stock);
    }
}
