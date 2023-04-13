using Microsoft.EntityFrameworkCore;
using SportsOrganizer.Data;

namespace SportsOrganizer.PostgreSqlMigrations;

public class PostgreSqlDbContextFactory : ApplicationDbContextFactory
{
    private readonly string _connectionString;

    public PostgreSqlDbContextFactory(string connectionString = "Host=localhost;Username=postgres;Password=root;Database=ef_core_testing")
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        base.OnConfiguring(optionsBuilder);
    }
}
