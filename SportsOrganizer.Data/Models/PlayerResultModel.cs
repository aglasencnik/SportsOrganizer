using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsOrganizer.Data.Models;

[Table("PlayerResults")]
public class PlayerResultModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ActivityResultId { get; set; }

    [Required]
    public double Result { get; set; }

    public ActivityResultModel ActivityResult { get; set; } = null!;
}
