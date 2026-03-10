using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAlert.Domain.Entities;

namespace StockAlert.Infrastructure.Data.Mappings
{
    public class AlertRuleMapping : IEntityTypeConfiguration<AlertRule>
    {
        public void Configure(EntityTypeBuilder<AlertRule> builder)
        {
             // tabela
             builder.ToTable("AlertRules");

            // chave primária
            builder.HasKey(a => a.Id);

            // simbolo de ação
            builder.Property(a => a.StockSymbol)
                .IsRequired()
                .HasMaxLength(20);

            // preço alvo
            builder.Property(a => a.TargetPrice)
                .HasColumnType("decimal(18,4)")
                .IsRequired(false);

            // variação percentual
            builder.Property(a => a.PercentageChange)
                .HasColumnType("decimal(10,4)")
                .IsRequired(false);

            // operador de comparação
            builder.Property(a => a.Operator)
                .HasConversion<int>()
                .IsRequired();

            // canal de notificação preferido
            builder.Property(a => a.PreferredChannel)
                .HasConversion<int>()
                .IsRequired(false);

            // flags 
            builder.Property(a => a.IsActive)
                .HasDefaultValue(true);

            builder.Property(a => a.NotifyOnce)
                .HasDefaultValue(false);

            // cooldown
            builder.Property(a => a.CooldownMinutes)
                .HasDefaultValue(15);

            // timestamps
            builder.Property(a => a.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()");

            builder.Property(a => a.LastTriggeredAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired(false);

            builder.Property(a => a.DeletedAt)
               .HasColumnType("timestamp with time zone")
               .IsRequired(false);

            // filtro global para soft delete
            builder.HasQueryFilter(a => EF.Property<DateTime?>(a, "DeletedAt") == null);
            
            // indices importantes
            builder.HasIndex(a => a.UserId)
                .HasDatabaseName("ix_alertRules_userId");
            builder.HasIndex(a => a.StockSymbol)
                .HasDatabaseName("ix_alertRules_stockSymbol");
            builder.HasIndex(a => new { a.UserId, a.StockSymbol })
                .HasDatabaseName("ix_alertRules_userId_stockSymbol");

            //relacionamento com usuário
            builder.HasOne(a => a.User)
                .WithMany(u => u.Alerts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // relacionamento com histórico de notificações
            builder.HasMany(a => a.NotificationHistory)
                .WithOne(n => n.AlertRule)
                .HasForeignKey(n => n.AlertRuleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
