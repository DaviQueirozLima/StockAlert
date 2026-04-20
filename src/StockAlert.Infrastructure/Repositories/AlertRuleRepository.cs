using Microsoft.EntityFrameworkCore;
using StockAlert.Domain.Entities;
using StockAlert.Domain.Repositories;
using StockAlert.Infrastructure.Data;

namespace StockAlert.Infrastructure.Repositories
{
    public class AlertRuleRepository : IAlertRuleRepository
    {
        private readonly AppDbContext _dbContext;
        public AlertRuleRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(AlertRule alertRule)
        {
            await _dbContext.AlertRules.AddAsync(alertRule);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            return await _dbContext.AlertRules
             .AsNoTracking()
             .Where(r => r.UserId == userId && r.DeletedAt == null)
             .ToListAsync();
        }

        public async Task<IEnumerable<AlertRule>> GetByUserIdAsync(Guid userId)
        {
            var rule = await _dbContext.AlertRules.FindAsync(id);
            if (rule != null)
            {
                // Implementando o Soft Delete: apenas marcamos a data de exclusão
                rule.DeletedAt = DateTime.UtcNow;
                rule.IsActive = false;
                _dbContext.AlertRules.Update(rule);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
