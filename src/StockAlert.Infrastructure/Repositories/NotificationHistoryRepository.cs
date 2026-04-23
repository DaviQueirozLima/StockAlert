using StockAlert.Domain.Entities;
using StockAlert.Domain.Repositories;
using StockAlert.Infrastructure.Data;

namespace StockAlert.Infrastructure.Repositories
{
    public class NotificationHistoryRepository : INotificationHistoryRepository
    {

        private readonly AppDbContext _dbContext;
        public NotificationHistoryRepository(AppDbContext dbContext)
        {
          _dbContext = dbContext;
        }

        public async Task AddAsync(NotificationHistory history)
        {
            await _dbContext.NotificationHistories.AddAsync(history);
            await _dbContext.SaveChangesAsync();
        }
    }
}
