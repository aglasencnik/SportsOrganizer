using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportsOrganizer.Data.Models;

[Table("UserActivities")]
public class UserActivityModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int ActivityId { get; set; }

    public UserModel User { get; set; } = null!;
    public ActivityModel Activity { get; set; } = null!;
}
