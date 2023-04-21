using SportsOrganizer.Data.Models;

namespace SportsOrganizer.Server.Models;

public class ActivityResultScoresModel
{
    public int Place { get; set; }
    public int Points { get; set; }
    public TeamModel Team { get; set; }
    public List<int> ActivityPlaces { get; set; }
}
