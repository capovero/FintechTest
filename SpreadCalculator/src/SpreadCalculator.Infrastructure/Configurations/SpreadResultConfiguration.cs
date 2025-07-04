using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpreadCalculator.Domain.Entities;

public class SpreadResultConfiguration : IEntityTypeConfiguration<SpreadResult>
{
    public void Configure(EntityTypeBuilder<SpreadResult> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.NearPrice)
            .HasColumnType("decimal(18,8)");

        builder.Property(x => x.FarPrice)
            .HasColumnType("decimal(18,8)");

        builder.Ignore(x => x.Spread); 

        builder.Property(x => x.Timestamp)
            .IsRequired();
    }
}