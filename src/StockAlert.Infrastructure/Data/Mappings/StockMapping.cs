using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAlert.Domain.Entities;

namespace StockAlert.Infrastructure.Data.Mappings
{
    public class StockMapping : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            // tabela
            builder.ToTable("Stocks");

            // chave primária
            builder.HasKey(s => s.Symbol);

            // símbolo da ação
            builder.Property(s => s.Symbol)
                .IsRequired()
                .HasMaxLength(20);

            // preço atual
            builder.Property(s => s.CurrentPrice)
                .HasColumnType("decimal(18,4)")
                .IsRequired();

            // última atualização do preço
            builder.Property(s => s.LastUpdated)
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("NOW()");

            // fonte do preço
            builder.Property(s => s.Source)
                .HasMaxLength(100)
                .IsRequired(false);

            // índice para consultas rápidas por símbolo
            builder.HasIndex(s => s.Symbol)
                .HasDatabaseName("ix_stocks_symbol");
        }
    }
}
