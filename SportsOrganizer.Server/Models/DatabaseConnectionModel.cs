using SportsOrganizer.Server.Enums;

namespace SportsOrganizer.Server.Models;

public class DatabaseConnectionModel
{
    public DatabaseProviderType DatabaseProvider { get; set; }
    public string ConnectionString { get; set; }
}
