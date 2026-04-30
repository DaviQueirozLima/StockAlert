using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockAlert.Domain.Entities;

namespace StockAlert.Infrastructure.Data.Mappings
{
    public class RoleMapping : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            // chave primária
            builder.HasKey(r => r.Id);

            // nome da role (enum)
            builder.Property(r => r.Name)
                .HasConversion<int>()
                .IsRequired();

            // índice para evitar roles duplicadas
            builder.HasIndex(r => r.Name)
                .IsUnique()
                .HasDatabaseName("ix_roles_name");
        }
    }
}
