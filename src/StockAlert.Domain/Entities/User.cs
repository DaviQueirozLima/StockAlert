namespace StockAlert.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!; 

        // Identificador do provedor OAuth (Google)
        public string? GoogleId { get; set; }

        // Opcional: caso eu adicione whatsapp ou outro canal no futuro, posso usar esse campo para armazenar o número de telefone
        public string? PhoneNumber { get; set; }

        // opcional: caso queira implementar autenticação local, pode usar esse campo para armazenar o hash da senha (recomendado usar uma biblioteca como BCrypt)
        public string? PasswordHash { get; set; }

        // Preferências de notificação do usuário (podem ser sobrescritas por AlertRule)
        public bool NotifyByEmail { get; set; } = true;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }

        // Navegação
        public ICollection<AlertRule> Alerts { get; set; } = new List<AlertRule>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<NotificationHistory> NotificationHistory { get; set; } = new List<NotificationHistory>();
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();

        // Role opcional
        public Guid? RoleId { get; set; }
        public Role? Role { get; set; }

    }
}
