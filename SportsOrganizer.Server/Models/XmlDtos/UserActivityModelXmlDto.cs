using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportsOrganizer.Server.Models.XmlDtos;

public class UserActivityModelXmlDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ActivityId { get; set; }
    public ActivityModelXmlDto Activity { get; set; }
}
