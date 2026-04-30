using StockAlert.Domain.Enums;

namespace StockAlert.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public RoleType Name { get; set; } = RoleType.User;
    }
}
