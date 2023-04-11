using SportsOrganizer.Server.Enums;

namespace SportsOrganizer.Server.Models;

public class AppSettingsModel
{
    public int Id { get; set; }
    public KeyValueType KeyValueType { get; set; }
    public string Value { get; set; } = string.Empty;
    public byte[] ImageData { get; set; }
    public bool BoolValue { get; set; }
}
