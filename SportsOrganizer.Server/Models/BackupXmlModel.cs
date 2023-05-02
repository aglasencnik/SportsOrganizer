using SportsOrganizer.Server.Models.XmlDtos;

namespace SportsOrganizer.Server.Models;

public class BackupXmlModel
{
    public List<AppSettingsModel> AppSettings { get; set; }
    public List<TeamModelXmlDto> Teams { get; set; }
    public List<ActivityModelXmlDto> Activities { get; set; }
    public List<UserModelXmlDto> Users { get; set; }
}
