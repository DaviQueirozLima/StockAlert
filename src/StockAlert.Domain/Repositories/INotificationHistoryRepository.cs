using StockAlert.Domain.Entities;

namespace StockAlert.Domain.Repositories
{
    public interface INotificationHistoryRepository
    {   
        Task AddAsync(NotificationHistory history);
    }
}
