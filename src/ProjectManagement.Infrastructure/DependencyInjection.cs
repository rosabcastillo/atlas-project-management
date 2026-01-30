using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Application.Services;
using ProjectManagement.Infrastructure.Data;
using ProjectManagement.Infrastructure.Repositories;

namespace ProjectManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<AllocationService>();

        return services;
    }
}
