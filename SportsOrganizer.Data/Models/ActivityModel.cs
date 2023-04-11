using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SportsOrganizer.Data.Enums;

namespace SportsOrganizer.Data.Models;

[Table("Activities")]
public class ActivityModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ActivityNumber { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Location { get; set; } = null!;

    [Required]
    public string Rules { get; set; } = null!;

    [Required]
    public string Props { get; set; } = null!;

    [Required]
    public ActivityType ActivityType { get; set; }

    [Required]
    public OrderType OrderType { get; set; }

    [Required]
    public int NumberOfPlayers { get; set; }

    public ICollection<UserActivityModel> UserActivities { get; set; } = null!;
}
