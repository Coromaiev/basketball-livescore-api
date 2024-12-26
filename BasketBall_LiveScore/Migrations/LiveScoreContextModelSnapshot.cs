﻿// <auto-generated />
using System;
using BasketBall_LiveScore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BasketBall_LiveScore.Migrations
{
    [DbContext(typeof(LiveScoreContext))]
    partial class LiveScoreContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BasketBall_LiveScore.Models.Match", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<decimal>("HostsId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("HostsScore")
                        .HasColumnType("decimal(20,0)");

                    b.Property<byte>("NumberOfQuarters")
                        .HasColumnType("tinyint");

                    b.Property<decimal>("PlayEncoderId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("PrepEncoderId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<byte>("QuarterDuration")
                        .HasColumnType("tinyint");

                    b.Property<byte>("TimeOutDuration")
                        .HasColumnType("tinyint");

                    b.Property<decimal>("VisitorsId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("VisitorsScore")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("HostsId");

                    b.HasIndex("PlayEncoderId");

                    b.HasIndex("PrepEncoderId");

                    b.HasIndex("VisitorsId");

                    b.ToTable("Matchs");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.MatchEvent", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.Property<decimal>("MatchId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<byte>("QuarterNumber")
                        .HasColumnType("tinyint");

                    b.Property<TimeSpan>("Time")
                        .HasColumnType("time");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.ToTable("MatchEvents");

                    b.HasDiscriminator().HasValue("MatchEvent");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.Player", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<byte>("Number")
                        .HasColumnType("tinyint");

                    b.Property<decimal>("TeamId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.Team", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.User", b =>
                {
                    b.Property<decimal>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<decimal>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Permission")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MatchPlayer", b =>
                {
                    b.Property<decimal>("HostsStartingPlayersId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("MatchId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("HostsStartingPlayersId", "MatchId");

                    b.HasIndex("MatchId");

                    b.ToTable("MatchPlayer");
                });

            modelBuilder.Entity("MatchPlayer1", b =>
                {
                    b.Property<decimal>("Match1Id")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("VisitorsStartingPlayersId")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Match1Id", "VisitorsStartingPlayersId");

                    b.HasIndex("VisitorsStartingPlayersId");

                    b.ToTable("MatchPlayer1");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.Fault", b =>
                {
                    b.HasBaseType("BasketBall_LiveScore.Models.MatchEvent");

                    b.Property<decimal>("FaultyPlayerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasIndex("FaultyPlayerId");

                    b.HasDiscriminator().HasValue("Fault");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.PlayerChange", b =>
                {
                    b.HasBaseType("BasketBall_LiveScore.Models.MatchEvent");

                    b.Property<decimal>("LeavingPlayerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("ReplacingPlayerId")
                        .HasColumnType("decimal(20,0)");

                    b.HasIndex("LeavingPlayerId");

                    b.HasIndex("ReplacingPlayerId");

                    b.HasDiscriminator().HasValue("PlayerChange");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.ScoreChange", b =>
                {
                    b.HasBaseType("BasketBall_LiveScore.Models.MatchEvent");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.Property<decimal>("ScorerId")
                        .HasColumnType("decimal(20,0)");

                    b.HasIndex("ScorerId");

                    b.HasDiscriminator().HasValue("ScoreChange");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.TimeOut", b =>
                {
                    b.HasBaseType("BasketBall_LiveScore.Models.MatchEvent");

                    b.Property<decimal>("InvokerId")
                        .HasColumnType("decimal(20,0)");

                    b.HasIndex("InvokerId");

                    b.HasDiscriminator().HasValue("TimeOut");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.Match", b =>
                {
                    b.HasOne("BasketBall_LiveScore.Models.Team", "Hosts")
                        .WithMany()
                        .HasForeignKey("HostsId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BasketBall_LiveScore.Models.User", "PlayEncoder")
                        .WithMany()
                        .HasForeignKey("PlayEncoderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BasketBall_LiveScore.Models.User", "PrepEncoder")
                        .WithMany()
                        .HasForeignKey("PrepEncoderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BasketBall_LiveScore.Models.Team", "Visitors")
                        .WithMany()
                        .HasForeignKey("VisitorsId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Hosts");

                    b.Navigation("PlayEncoder");

                    b.Navigation("PrepEncoder");

                    b.Navigation("Visitors");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.MatchEvent", b =>
                {
                    b.HasOne("BasketBall_LiveScore.Models.Match", "Match")
                        .WithMany("Events")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Match");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.Player", b =>
                {
                    b.HasOne("BasketBall_LiveScore.Models.Team", "Team")
                        .WithMany("Players")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("MatchPlayer", b =>
                {
                    b.HasOne("BasketBall_LiveScore.Models.Player", null)
                        .WithMany()
                        .HasForeignKey("HostsStartingPlayersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BasketBall_LiveScore.Models.Match", null)
                        .WithMany()
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MatchPlayer1", b =>
                {
                    b.HasOne("BasketBall_LiveScore.Models.Match", null)
                        .WithMany()
                        .HasForeignKey("Match1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BasketBall_LiveScore.Models.Player", null)
                        .WithMany()
                        .HasForeignKey("VisitorsStartingPlayersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.Fault", b =>
                {
                    b.HasOne("BasketBall_LiveScore.Models.Player", "FaultyPlayer")
                        .WithMany()
                        .HasForeignKey("FaultyPlayerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("FaultyPlayer");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.PlayerChange", b =>
                {
                    b.HasOne("BasketBall_LiveScore.Models.Player", "LeavingPlayer")
                        .WithMany()
                        .HasForeignKey("LeavingPlayerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BasketBall_LiveScore.Models.Player", "ReplacingPlayer")
                        .WithMany()
                        .HasForeignKey("ReplacingPlayerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("LeavingPlayer");

                    b.Navigation("ReplacingPlayer");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.ScoreChange", b =>
                {
                    b.HasOne("BasketBall_LiveScore.Models.Player", "Scorer")
                        .WithMany()
                        .HasForeignKey("ScorerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Scorer");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.TimeOut", b =>
                {
                    b.HasOne("BasketBall_LiveScore.Models.Team", "Invoker")
                        .WithMany()
                        .HasForeignKey("InvokerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Invoker");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.Match", b =>
                {
                    b.Navigation("Events");
                });

            modelBuilder.Entity("BasketBall_LiveScore.Models.Team", b =>
                {
                    b.Navigation("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
