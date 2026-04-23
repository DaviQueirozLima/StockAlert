using StockAlert.Domain.Entities;

namespace StockAlert.Domain.Repositories
{
    public interface IAlertRuleRepository
    {
        Task AddAsync(AlertRule alertRule);
        Task UpdateAsync(AlertRule alertRule);
        Task<IEnumerable<AlertRule>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<AlertRule>> GetAllActiveAsync();
        Task DeleteAsync(Guid id); 
    }
}
