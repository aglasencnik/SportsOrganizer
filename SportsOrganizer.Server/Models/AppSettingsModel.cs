using SportsOrganizer.Server.Enums;

namespace SportsOrganizer.Server.Models;

public class AppSettingsModel
{
    public int Id { get; set; }
    public KeyValueType KeyValueType { get; set; }
    public object Value { get; set; }
}
