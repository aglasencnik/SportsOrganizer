﻿using SportsOrganizer.Data.Models;

namespace SportsOrganizer.Server.Models;

public class ActivityResultSortModel
{
    public int Place { get; set; }
    public TeamModel? Team { get; set; }
    public ActivityModel? Activity { get; set; }
    public ActivityResultModel? ActivityResult { get; set; }
    public List<PlayerResultModel>? PlayerResults { get; set; }
}
