using StockAlert.Domain.Enums;

namespace StockAlert.Domain.Entities
{
    public class AlertRule
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK para User
        public Guid UserId { get; set; }
        public User? User { get; set; }

        // Símbolo da ação (ex: "AAPL", "PETR4")
        public string StockSymbol { get; set; } = default!;

        // Condição da regra: preço alvo ou variação percentual
        public decimal? TargetPrice { get; set; }    // ex: 35.00
        public decimal? PercentageChange { get; set; } // ex: 5 (para 5%)

        public ComparisonOperator Operator { get; set; } // >, <, >=, ...

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastTriggeredAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Histórico de notificações associadas
        public ICollection<NotificationHistory> NotificationHistory { get; set; } = new List<NotificationHistory>();

        // Preferência de canal para esta regra (se null, usa preferência do User)
        public NotificationChannel? PreferredChannel { get; set; }

        // Controle de spam/duplicatas:
        // cooldown em minutos desde o último disparo antes de permitir novo disparo
        public int? CooldownMinutes { get; set; } = 15;

        // Se true, disparar apenas uma vez e desativar depois
        public bool NotifyOnce { get; set; } = false;
    }
}
