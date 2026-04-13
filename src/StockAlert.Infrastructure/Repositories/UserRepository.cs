using Microsoft.EntityFrameworkCore;
using StockAlert.Domain.Entities;
using StockAlert.Domain.Repositories;
using StockAlert.Infrastructure.Data;

namespace StockAlert.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Add(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetByGoogleIdAsync(string googleId)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.GoogleId == googleId);
        }
    }
}
