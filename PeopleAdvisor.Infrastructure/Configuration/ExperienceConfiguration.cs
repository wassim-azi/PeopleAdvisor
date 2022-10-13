using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeopleAdvisor.Domain.Entities;

namespace PeopleAdvisor.Infrastructure.Configuration;

public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
{
    public void Configure(EntityTypeBuilder<Experience> builder)
    {
        builder.ToTable("Experience");
        builder.HasKey(k => k.Id);
        builder.Property(c => c.Company).HasMaxLength(128);
        builder.Property(e => e.EndDate).HasColumnType("Date");
        builder.Property(i => i.IsPrimary);
        builder.Property(s => s.StartDate).HasColumnType("Date");
        builder.Property(t => t.Title).HasMaxLength(254);
    }
}