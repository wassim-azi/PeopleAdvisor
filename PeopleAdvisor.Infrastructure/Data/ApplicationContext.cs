using Microsoft.EntityFrameworkCore;
using PeopleAdvisor.Domain.Entities;

namespace PeopleAdvisor.Infrastructure.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Profile>? Profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // A new extension method, ApplyConfigurationsFromAssembly, was introduced which scans a given
        // assembly for all types that implement IEntityTypeConfiguration, and registers each one automatically.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
    }
}