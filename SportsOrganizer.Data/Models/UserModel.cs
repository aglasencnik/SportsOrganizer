using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SportsOrganizer.Data.Enums;

namespace SportsOrganizer.Data.Models;

[Table("Users")]
public class UserModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public UserType UserType { get; set; }

    public ICollection<UserActivityModel> UserActivities { get; set; } = null!;
}
