using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PeopleAdvisor.Domain.Entities;

namespace PeopleAdvisor.Infrastructure.Configuration;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ToTable("Profile");
        builder.HasKey(k => k.Id);
        builder.Property(a => a.Age);
        builder.Property(c => c.City).HasMaxLength(128);
        builder.Property(c => c.Country).HasMaxLength(128);
        builder.Property(c => c.CreationDate).HasColumnType("Date");
        builder.Property(c => c.Currency).HasMaxLength(64);
        builder.Property(c => c.CurrentSalary);
        builder.Property(d => d.Degree).HasMaxLength(254);
        builder.Property(e => e.Email).HasMaxLength(254);
        builder.Property(e => e.ExperienceYears);
        builder.Property(f => f.FirstName).HasMaxLength(64);
        builder.Property(g => g.Gender).HasMaxLength(16);
        builder.Property(l => l.Languages).HasMaxLength(128);
        builder.Property(l => l.LastName).HasMaxLength(64);
        builder.Property(l => l.Linkedin).HasMaxLength(128);
        builder.Property(n => n.NoticePeriod).HasMaxLength(128);
        builder.Property(p => p.ParsedResume);
        builder.Property(p => p.PhoneNumber).HasMaxLength(16);
        builder.Property(r => r.RemoteOnly);
        builder.Property(r => r.Role).HasMaxLength(254);
        builder.Property(s => s.School).HasMaxLength(254);
        builder.Property(s => s.Sector).HasMaxLength(254);
        builder.Property(s => s.Skills);
        builder.Property(s => s.StudyLevel).HasMaxLength(254);
        builder.Property(u => u.UpdateDate).HasColumnType("Date");

        builder
            .HasMany<Experience>(e => e.Experiences)
            .WithOne(s => s.Profile)
            .HasForeignKey(s => s.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}