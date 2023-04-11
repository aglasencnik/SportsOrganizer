using Microsoft.EntityFrameworkCore;
using SportsOrganizer.Data.Models;

namespace SportsOrganizer.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<TeamModel> Teams { get; set; }
    public DbSet<UserModel> Users { get; set; }
    public DbSet<ActivityModel> Activities { get; set; }
    public DbSet<UserActivityModel> UserActivities { get; set; }
    public DbSet<ActivityResultModel> ActivityResults { get; set; }
    public DbSet<PlayerResultModel> PlayerResults { get; set; }
}
