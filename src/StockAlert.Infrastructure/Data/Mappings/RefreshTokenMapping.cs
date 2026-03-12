using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAlert.Domain.Entities;

namespace StockAlert.Infrastructure.Data.Mappings
{
    public class RefreshTokenMapping : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // tabela
            builder.ToTable("RefreshTokens");

            // chave primária
            builder.HasKey(r => r.Id);

            // token
            builder.Property(r => r.Token)
                .IsRequired()
                .HasMaxLength(500);

            // data de expiração
            builder.Property(r => r.ExpiresAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            // data de criação
            builder.Property(r => r.CreatedAt)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()");

            // data de revogação
            builder.Property(r => r.RevokedAt)
                .HasColumnType("timestamp with time zone")
                .IsRequired(false);

            // token substituto (rotation)
            builder.Property(r => r.ReplacedByToken)
                .HasMaxLength(500)
                .IsRequired(false);

            // índices importantes
            builder.HasIndex(r => r.Token)
                .HasDatabaseName("ix_refreshtokens_token")
                .IsUnique();

            builder.HasIndex(r => r.UserId)
                .HasDatabaseName("ix_refreshtokens_userid");

            builder.HasIndex(r => r.ExpiresAt)
                .HasDatabaseName("ix_refreshtokens_expiresat");

            // relacionamento com usuário
            builder.HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
