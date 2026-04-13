using StockAlert.Domain.Entities;

namespace StockAlert.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByGoogleIdAsync(string googleId);
        Task Add(User user);
    }
}
