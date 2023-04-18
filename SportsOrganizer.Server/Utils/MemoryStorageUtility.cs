using SportsOrganizer.Server.Enums;
using SportsOrganizer.Server.Models;

namespace SportsOrganizer.Server.Utils;

public class MemoryStorageUtility
{
    public Dictionary<KeyValueType, object> Storage { get; set; } = new();

    public void SetValuesFromLiteDb(IEnumerable<AppSettingsModel> appSettings)
    {
        foreach (var setting in appSettings)
        {
            Storage[setting.KeyValueType] = setting.Value;
        }
    }

    public void SetValue(KeyValueType Key, object Value)
    {
        Storage[Key] = Value;
    }

    public object GetValue(KeyValueType Key)
    {
        Storage.TryGetValue(Key, out var value);
        return value;
    }

    public void Remove(KeyValueType Key)
    {
        Storage.Remove(Key);
    }

    public void ClearAll()
    {
        Storage.Clear();
    }
}
