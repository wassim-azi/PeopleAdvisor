using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeopleAdvisor.Domain.Interfaces;
using PeopleAdvisor.Infrastructure.Repositories;

namespace PeopleAdvisor.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

        return services;
    }
}