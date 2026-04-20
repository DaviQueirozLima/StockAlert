using StockAlert.Domain.Entities;

namespace StockAlert.Domain.Repositories
{
    public interface IAlertRuleRepository
    {
        Task AddAsync(AlertRule alertRule);
        Task<IEnumerable<AlertRule>> GetByUserIdAsync(Guid userId);
        Task DeleteAsync(Guid id); // Para o Soft Delete futuramente
    }
}
