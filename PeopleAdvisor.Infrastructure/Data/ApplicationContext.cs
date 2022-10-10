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
        // https://stackoverflow.com/a/32859508
        // modelBuilder.Configurations.Add(new TestEntity1Configuration());
        // modelBuilder.Configurations.Add(new TestEntity2Configuration());

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
    }
}