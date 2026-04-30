namespace StockAlert.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }

        // se substituído por outro refresh token
        public string? ReplacedByToken { get; set; }

        // FK para User
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
