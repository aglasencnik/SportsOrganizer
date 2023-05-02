using SportsOrganizer.Data.Enums;

namespace SportsOrganizer.Server.Models.XmlDtos;

public class UserModelXmlDto
{
    public int Id { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public UserType UserType { get; set; }

    public List<UserActivityModelXmlDto> AssignedActivities { get; set; }
}
