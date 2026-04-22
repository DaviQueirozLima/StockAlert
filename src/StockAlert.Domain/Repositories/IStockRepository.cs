using StockAlert.Domain.Entities;

namespace StockAlert.Domain.Repositories
{
    public interface IStockRepository
    {
        Task<Stock?> GetBySymbolAndUserIdAsync(string symbol, Guid userId);
        Task<IEnumerable<Stock>> GetAllAsync();
        Task AddAsync(Stock stock);
        Task UpdateAsync(Stock stock);
    }
}
