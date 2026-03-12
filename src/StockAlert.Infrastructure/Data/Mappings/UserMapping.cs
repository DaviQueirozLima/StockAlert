using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAlert.Domain.Entities;
using System;

namespace StockAlert.Infrastructure.Data.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // tabela
            builder.ToTable("Users");

            // chave primária
            builder.HasKey(u => u.Id);

            // campos principais
            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.GoogleId)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(u => u.TelegramChatId)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(u => u.PasswordHash)
                .HasMaxLength(300)
                .IsRequired(false);

            // flags booleanas (defaults)
            builder.Property(u => u.IsActive).HasDefaultValue(true);
            builder.Property(u => u.IsTelegramVerified).HasDefaultValue(false);
            builder.Property(u => u.NotifyByEmail).HasDefaultValue(true);
            builder.Property(u => u.NotifyByTelegram).HasDefaultValue(true);

            // timestamps
            builder.Property(u => u.CreatedAt)
                .HasDefaultValueSql("NOW()")
                .HasColumnType("timestamp with time zone");

            // soft delete / auditoria
            builder.Property(u => u.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired(false);


            // filtro global para soft delete: todas as queries ignoram registros com DeletedAt != null
            builder.HasQueryFilter(u => EF.Property<DateTime?>(u, "DeletedAt") == null);

            // índices com nomes explícitos
            builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("ux_users_email");
            builder.HasIndex(u => u.GoogleId).IsUnique().HasDatabaseName("ux_users_google_id");
            builder.HasIndex(u => u.TelegramChatId).IsUnique().HasDatabaseName("ux_users_telegram_chat_id").HasFilter("\"TelegramChatId\" IS NOT NULL");

            // relacionamentos
            builder.HasMany(u => u.Alerts)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.NotificationHistory)
                .WithOne(nh => nh.User)
                .HasForeignKey(nh => nh.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}