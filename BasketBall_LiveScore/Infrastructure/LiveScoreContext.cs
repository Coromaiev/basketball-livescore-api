using BasketBall_LiveScore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BasketBall_LiveScore.Infrastructure
{
    public class LiveScoreContext : DbContext
    {
        public LiveScoreContext(DbContextOptions<LiveScoreContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matchs { get; set; }
        public DbSet<MatchEvent> MatchEvents { get; set; }
        public DbSet<Fault> Faults { get; set; }
        public DbSet<TimeOut> TimeOuts { get; set; }
        public DbSet<ScoreChange> ScoreChanges { get; set; }
        public DbSet<PlayerChange> PlayerChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fault>().HasBaseType<MatchEvent>();
            modelBuilder.Entity<TimeOut>().HasBaseType<MatchEvent>();
            modelBuilder.Entity<ScoreChange>().HasBaseType<MatchEvent>();
            modelBuilder.Entity<PlayerChange>().HasBaseType<MatchEvent>();

            modelBuilder.Entity<Match>()
                .HasOne(match => match.PlayEncoder)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(match => match.PrepEncoder)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(match => match.Hosts)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Match>()
                .HasMany(match => match.HostsStartingPlayers)
                .WithMany();

            modelBuilder.Entity<Match>()
                .HasMany(match => match.VisitorsStartingPlayers)
                .WithMany();

            modelBuilder.Entity<Match>()
                .HasOne(match => match.Visitors)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MatchEvent>()
                .HasOne(matchEvent => matchEvent.Match)
                .WithMany(match => match.Events)
                .HasForeignKey(matchEvent => matchEvent.MatchId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PlayerChange>()
                .HasOne(change => change.LeavingPlayer)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PlayerChange>()
                .HasOne(change => change.ReplacingPlayer)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ScoreChange>()
                .HasOne(scoreChange => scoreChange.Scorer)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TimeOut>()
                .HasOne(timeOut => timeOut.Invoker)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Fault>()
                .HasOne(fault => fault.FaultyPlayer)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Player>()
                .HasOne(player => player.Team)
                .WithMany(team => team.Players)
                .HasForeignKey(player => player.TeamId)
                .OnDelete(DeleteBehavior.NoAction);
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
