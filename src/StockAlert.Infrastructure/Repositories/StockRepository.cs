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

    public async Task<Stock?> GetBySymbolAsync(string symbol)
    {
        // Buscamos a ação pelo símbolo (ex: PETR4)
        return await _dbContext.Stocks
            .FirstOrDefaultAsync(s => s.Symbol.ToUpper() == symbol.ToUpper());
    }

    public async Task AddAsync(Stock stock)
    {
        // Adiciona a nova ação monitorada ao banco
        await _dbContext.Stocks.AddAsync(stock);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Stock stock)
    {
        // Atualiza o preço e a data da última consulta
        _dbContext.Stocks.Update(stock);
        await _dbContext.SaveChangesAsync();
    }
}
