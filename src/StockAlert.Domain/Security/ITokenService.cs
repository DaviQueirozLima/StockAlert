using StockAlert.Domain.Entities;

namespace StockAlert.Domain.Security
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
