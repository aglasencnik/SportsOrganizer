using Microsoft.EntityFrameworkCore;
using SportsOrganizer.Data;

namespace SportsOrganizer.MySqlMigrations;

public class MySqlDbContextFactory : ApplicationDbContextFactory
{
    private readonly string _connectionString;

    public MySqlDbContextFactory(string connectionString = "Server=localhost;Database=ef_core_testing;Uid=root;Pwd=admin;")
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySQL(_connectionString);
        base.OnConfiguring(optionsBuilder);
    }
}
