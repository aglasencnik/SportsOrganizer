using SportsOrganizer.Data.Enums;

namespace SportsOrganizer.Server.Models.XmlDtos;

public class ActivityModelXmlDto
{
    public int Id { get; set; }

    public int ActivityNumber { get; set; }

    public string Title { get; set; }

    public string Location { get; set; }

    public string Rules { get; set; }

    public string Props { get; set; }

    public ActivityType ActivityType { get; set; }

    public OrderType OrderType { get; set; }

    public int NumberOfPlayers { get; set; }

    public List<ActivityResultModelXmlDto> ActivityResults { get; set; }
}
