using StockAlert.Domain.Enums;

namespace StockAlert.Domain.Entities
{
    public class NotificationHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK para AlertRule
        public Guid AlertRuleId { get; set; }
        public AlertRule? AlertRule { get; set; }

        // FK para User
        public Guid? UserId { get; set; }
        public User? User { get; set; }

        // Quando foi enviado
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // Canal usado
        public NotificationChannel Channel { get; set; }

        // Resultado do envio
        public bool Success { get; set; }

        // Status textual do envio (ex: "Sent", "Failed", "Queued")
        public string Status { get; set; } = default!;

        // Mensagem enviada (texto do alerta) — útil para auditoria
        public string Message { get; set; } = default!;

        // Destinatário exato usado: email ou telefone
        public string Recipient { get; set; } = default!;

        // Id externo retornado pelo provedor (opcional)
        public string? ExternalId { get; set; }
    }
}
