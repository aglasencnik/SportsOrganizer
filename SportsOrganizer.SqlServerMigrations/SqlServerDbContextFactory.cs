using Microsoft.EntityFrameworkCore;
using SportsOrganizer.Data;

namespace SportsOrganizer.SqlServerMigrations;

public class SqlServerDbContextFactory : ApplicationDbFactory
{
    private readonly string _connectionString;

    public SqlServerDbContextFactory(string connectionString = "Server=.;Database=ef_core_migration_testing;Encrypt=False;")
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
        base.OnConfiguring(optionsBuilder);
    }
}
