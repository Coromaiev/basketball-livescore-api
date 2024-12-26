using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasketBall_LiveScore.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Permission = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TeamId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Number = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matchs",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuarterDuration = table.Column<byte>(type: "tinyint", nullable: false),
                    NumberOfQuarters = table.Column<byte>(type: "tinyint", nullable: false),
                    TimeOutDuration = table.Column<byte>(type: "tinyint", nullable: false),
                    VisitorsId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    HostsId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    PrepEncoderId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    PlayEncoderId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    HostsScore = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    VisitorsScore = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matchs_Teams_HostsId",
                        column: x => x.HostsId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Matchs_Teams_VisitorsId",
                        column: x => x.VisitorsId,
                        principalTable: "Teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Matchs_Users_PlayEncoderId",
                        column: x => x.PlayEncoderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matchs_Users_PrepEncoderId",
                        column: x => x.PrepEncoderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchEvents",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    QuarterNumber = table.Column<byte>(type: "tinyint", nullable: false),
                    MatchId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: true),
                    FaultyPlayerId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    LeavingPlayerId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    ReplacingPlayerId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true),
                    ScorerId = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    InvokerID = table.Column<decimal>(type: "decimal(20,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchEvents_Matchs_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchEvents_Players_FaultyPlayerId",
                        column: x => x.FaultyPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatchEvents_Players_LeavingPlayerId",
                        column: x => x.LeavingPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatchEvents_Players_ReplacingPlayerId",
                        column: x => x.ReplacingPlayerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatchEvents_Players_ScorerId",
                        column: x => x.ScorerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatchEvents_Teams_InvokerID",
                        column: x => x.InvokerID,
                        principalTable: "Teams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MatchPlayer",
                columns: table => new
                {
                    HostsStartingPlayersId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    MatchId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchPlayer", x => new { x.HostsStartingPlayersId, x.MatchId });
                    table.ForeignKey(
                        name: "FK_MatchPlayer_Matchs_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchPlayer_Players_HostsStartingPlayersId",
                        column: x => x.HostsStartingPlayersId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchPlayer1",
                columns: table => new
                {
                    Match1Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    VisitorsStartingPlayersId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchPlayer1", x => new { x.Match1Id, x.VisitorsStartingPlayersId });
                    table.ForeignKey(
                        name: "FK_MatchPlayer1_Matchs_Match1Id",
                        column: x => x.Match1Id,
                        principalTable: "Matchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchPlayer1_Players_VisitorsStartingPlayersId",
                        column: x => x.VisitorsStartingPlayersId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchEvents_FaultyPlayerId",
                table: "MatchEvents",
                column: "FaultyPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchEvents_InvokerID",
                table: "MatchEvents",
                column: "InvokerID");

            migrationBuilder.CreateIndex(
                name: "IX_MatchEvents_LeavingPlayerId",
                table: "MatchEvents",
                column: "LeavingPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchEvents_MatchId",
                table: "MatchEvents",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchEvents_ReplacingPlayerId",
                table: "MatchEvents",
                column: "ReplacingPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchEvents_ScorerId",
                table: "MatchEvents",
                column: "ScorerId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchPlayer_MatchId",
                table: "MatchPlayer",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchPlayer1_VisitorsStartingPlayersId",
                table: "MatchPlayer1",
                column: "VisitorsStartingPlayersId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchs_HostsId",
                table: "Matchs",
                column: "HostsId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchs_PlayEncoderId",
                table: "Matchs",
                column: "PlayEncoderId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchs_PrepEncoderId",
                table: "Matchs",
                column: "PrepEncoderId");

            migrationBuilder.CreateIndex(
                name: "IX_Matchs_VisitorsId",
                table: "Matchs",
                column: "VisitorsId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamId",
                table: "Players",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchEvents");

            migrationBuilder.DropTable(
                name: "MatchPlayer");

            migrationBuilder.DropTable(
                name: "MatchPlayer1");

            migrationBuilder.DropTable(
                name: "Matchs");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
