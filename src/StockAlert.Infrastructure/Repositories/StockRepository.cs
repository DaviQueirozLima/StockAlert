using Microsoft.EntityFrameworkCore;
using StockAlert.Domain.Entities;
using StockAlert.Domain.Repositories;
using StockAlert.Infrastructure.Data;

namespace StockAlert.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly AppDbContext _dbContext;

    public StockRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Mudança aqui: O filtro agora usa Symbol E UserId
    public async Task<Stock?> GetBySymbolAndUserIdAsync(string symbol, Guid userId)
    {
        return await _dbContext.Stocks
            .FirstOrDefaultAsync(s => s.Symbol.ToUpper() == symbol.ToUpper() && s.UserId == userId);
    }

    public async Task<IEnumerable<Stock>> GetAllAsync()
    {
        return await _dbContext.Stocks.ToListAsync();
    }

    public async Task AddAsync(Stock stock)
    {
        await _dbContext.Stocks.AddAsync(stock);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Stock stock)
    {
        _dbContext.Stocks.Update(stock);
        await _dbContext.SaveChangesAsync();
    }
}
