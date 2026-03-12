using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAlert.Domain.Entities;

namespace StockAlert.Infrastructure.Data.Mappings
{
    public class NotificationHistoryMapping : IEntityTypeConfiguration<NotificationHistory>
    {
        public void Configure(EntityTypeBuilder<NotificationHistory> builder)
        {
            // tabela
            builder.ToTable("NotificationHistories");

            // chave primária
            builder.HasKey(n => n.Id);

            // timestamp de envio
            builder.Property(n => n.SentAt)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()");

            // canal (enum)
            builder.Property(n => n.Channel)
                .HasConversion<int>()
                .IsRequired();

            // sucesso do envio
            builder.Property(n => n.Success)
                .IsRequired();

            // status textual
            builder.Property(n => n.Status)
                .IsRequired()
                .HasMaxLength(50);

            // mensagem enviada
            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(500);

            // destinatário
            builder.Property(n => n.Recipient)
                .IsRequired()
                .HasMaxLength(200);

            // id externo do provedor
            builder.Property(n => n.ExternalId)
                .HasMaxLength(200)
                .IsRequired(false);

            // índices importantes
            builder.HasIndex(n => n.AlertRuleId)
                .HasDatabaseName("ix_notificationhistory_alertruleid");

            builder.HasIndex(n => n.UserId)
                .HasDatabaseName("ix_notificationhistory_userid");

            builder.HasIndex(n => n.SentAt)
                .HasDatabaseName("ix_notificationhistory_sentat");

            // relacionamento com AlertRule
            builder.HasOne(n => n.AlertRule)
                .WithMany(a => a.NotificationHistory)
                .HasForeignKey(n => n.AlertRuleId)
                .OnDelete(DeleteBehavior.Restrict);

            // relacionamento com User
            builder.HasOne(n => n.User)
                .WithMany(u => u.NotificationHistory)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
