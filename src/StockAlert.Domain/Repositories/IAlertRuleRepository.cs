using StockAlert.Domain.Entities;

namespace StockAlert.Domain.Repositories
{
    public interface IAlertRuleRepository
    {
        Task AddAsync(AlertRule alertRule);
        Task UpdateAsync(AlertRule alertRule);
        Task<IEnumerable<AlertRule>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<AlertRule>> GetAllActiveAsync();
        Task<AlertRule?> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id); 
    }
}
