namespace SportsOrganizer.Server.Models.XmlDtos;

public class ActivityResultModelXmlDto
{
    public int Id { get; set; }

    public TeamModelXmlDto Team { get; set; }

    public int ActivityId { get; set; }

    public List<PlayerResultModelXmlDto> PlayerResults { get; set; }

    public double Result { get; set; }
}
