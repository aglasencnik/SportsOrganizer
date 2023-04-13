using Microsoft.EntityFrameworkCore;

namespace SportsOrganizer.Server.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrUpdateDbContext<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        where TContext : DbContext
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TContext>));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<TContext>(options => optionsAction(options));

        return services;
    }
}
