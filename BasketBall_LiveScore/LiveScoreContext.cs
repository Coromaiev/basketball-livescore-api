using BasketBall_LiveScore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BasketBall_LiveScore
{
    public class LiveScoreContext : DbContext
    {
        public LiveScoreContext(DbContextOptions<LiveScoreContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matchs { get; set; }
        public DbSet<MatchEvent> MatchEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }

    public class LiveScoreContextFactory : IDesignTimeDbContextFactory<LiveScoreContext>
    {
        public LiveScoreContext CreateDbContext(string[]? args = null)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var optionsBuilder = new DbContextOptionsBuilder<LiveScoreContext>();
            optionsBuilder
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);

            return new LiveScoreContext(optionsBuilder.Options);
        }
    }
}
