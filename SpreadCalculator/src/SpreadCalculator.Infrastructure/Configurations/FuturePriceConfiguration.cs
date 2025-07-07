using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpreadCalculator.Domain.Entities;

namespace SpreadCalculator.Infrastructure.Configurations
{
    public class FuturePriceConfiguration : IEntityTypeConfiguration<FuturePrice>
    {
        public void Configure(EntityTypeBuilder<FuturePrice> builder)
        {
            builder.ToTable("FuturePrices");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Symbol)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ContractCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Timestamp)
                .IsRequired();
        }
    }
}