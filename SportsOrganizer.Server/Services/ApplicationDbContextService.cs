using Microsoft.EntityFrameworkCore;
using SportsOrganizer.Data;
using SportsOrganizer.Server.Enums;

namespace SportsOrganizer.Server.Services;

public class ApplicationDbContextService
{
    private ApplicationDbContext _dbContext;

    public ApplicationDbContext GetDbContext() => _dbContext;

    public bool IsDbContextSet() => _dbContext != null;

    public void SetDbContext(string connectionString, DatabaseProviderType providerType)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        switch (providerType)
        {
            case DatabaseProviderType.SqlServer:
                optionsBuilder.UseSqlServer(connectionString);
                break;
            case DatabaseProviderType.MySQL:
                optionsBuilder.UseMySQL(connectionString);
                break;
            case DatabaseProviderType.PostgreSQL:
                optionsBuilder.UseNpgsql(connectionString);
                break;
            default:
                throw new InvalidOperationException("Invalid provider specified");
        }

        _dbContext = new ApplicationDbContext(optionsBuilder.Options);
    }
}
