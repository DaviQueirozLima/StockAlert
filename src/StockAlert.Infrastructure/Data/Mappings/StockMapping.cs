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

            // chave primária composta: Symbol e UserId
            builder.HasKey(s => new { s.Symbol, s.UserId }); 

            // símbolo da ação
            builder.Property(s => s.Symbol)
                .IsRequired()
                .HasMaxLength(20);

            // UserId
            builder.Property(s => s.UserId) 
                .IsRequired();

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

            // Relação com User (um User tem muitas Stocks)
            builder.HasOne(s => s.User) 
                .WithMany(u => u.Stocks) 
                .HasForeignKey(s => s.UserId) 
                .OnDelete(DeleteBehavior.Cascade);

            // índice para consultas rápidas por símbolo (ainda útil, mas agora com UserId)
            builder.HasIndex(s => s.Symbol) 
                .HasDatabaseName("ix_stocks_symbol");

            // Opcional: Adicionar um índice composto para Symbol e UserId para otimizar buscas por usuário e símbolo
            builder.HasIndex(s => new { s.Symbol, s.UserId })
                .HasDatabaseName("ix_stocks_symbol_userid")
                .IsUnique(); 
        }
    }
}
