﻿using BasketBall_LiveScore.Models;
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
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasMany(match => match.HostsStartingPlayers)
                .WithMany();

            modelBuilder.Entity<Match>()
                .HasMany(match => match.VisitorsStartingPlayers)
                .WithMany();

            modelBuilder.Entity<Match>()
                .HasOne(match => match.Visitors)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerChange>()
                .HasOne(change => change.LeavingPlayer)
                .WithMany()
                .HasForeignKey(change => change.LeavingPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlayerChange>()
                .HasOne(change => change.ReplacingPlayer)
                .WithMany()
                .HasForeignKey(change => change.ReplacingPlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScoreChange>()
                .HasOne(scoreChange => scoreChange.Scorer)
                .WithMany()
                .HasForeignKey(scoreChange => scoreChange.ScorerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TimeOut>()
                .HasOne(timeOut => timeOut.Invoker)
                .WithMany()
                .HasForeignKey(timeOut => timeOut.InvokerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Fault>()
                .HasOne(fault => fault.FaultyPlayer)
                .WithMany()
                .HasForeignKey(fault => fault.FaultyPlayerId)
                .OnDelete(DeleteBehavior.Restrict);
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
