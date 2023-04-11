using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SportsOrganizer.Data.Models;

[Table("ActivityResults")]
public class ActivityResultModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TeamId { get; set; }

    [Required]
    public int ActivityId { get; set; }

    [Required]
    public double Result { get; set; }

    public ICollection<PlayerResultModel> PlayerResults { get; set; } = null!;
    public TeamModel Team { get; set; } = null!;
    public ActivityModel Activity { get; set; } = null!;
}
