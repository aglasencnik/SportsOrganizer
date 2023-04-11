using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportsOrganizer.Data.Models;

[Table("Teams")]
public class TeamModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public ICollection<ActivityResultModel> ActivityResults { get; set; } = null!;
}
