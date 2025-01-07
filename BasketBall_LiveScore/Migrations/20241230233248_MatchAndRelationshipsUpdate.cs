using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasketBall_LiveScore.Migrations
{
    /// <inheritdoc />
    public partial class MatchAndRelationshipsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchs_Users_PlayEncoderId",
                table: "Matchs");

            migrationBuilder.RenameColumn(
                name: "PlayEncoderId",
                table: "Matchs",
                newName: "WinnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Matchs_PlayEncoderId",
                table: "Matchs",
                newName: "IX_Matchs_WinnerId");

            migrationBuilder.AddColumn<bool>(
                name: "HasStarted",
                table: "Matchs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Matchs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "MatchUser",
                columns: table => new
                {
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayEncodersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchUser", x => new { x.MatchId, x.PlayEncodersId });
                    table.ForeignKey(
                        name: "FK_MatchUser_Matchs_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchUser_Users_PlayEncodersId",
                        column: x => x.PlayEncodersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchUser_PlayEncodersId",
                table: "MatchUser",
                column: "PlayEncodersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchs_Teams_WinnerId",
                table: "Matchs",
                column: "WinnerId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matchs_Teams_WinnerId",
                table: "Matchs");

            migrationBuilder.DropTable(
                name: "MatchUser");

            migrationBuilder.DropColumn(
                name: "HasStarted",
                table: "Matchs");

            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Matchs");

            migrationBuilder.RenameColumn(
                name: "WinnerId",
                table: "Matchs",
                newName: "PlayEncoderId");

            migrationBuilder.RenameIndex(
                name: "IX_Matchs_WinnerId",
                table: "Matchs",
                newName: "IX_Matchs_PlayEncoderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matchs_Users_PlayEncoderId",
                table: "Matchs",
                column: "PlayEncoderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
