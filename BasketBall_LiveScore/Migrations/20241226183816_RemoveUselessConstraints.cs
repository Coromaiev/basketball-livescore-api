using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasketBall_LiveScore.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUselessConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchEvents_Matchs_MatchId",
                table: "MatchEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchEvents_Teams_InvokerID",
                table: "MatchEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "InvokerID",
                table: "MatchEvents",
                newName: "InvokerId");

            migrationBuilder.RenameIndex(
                name: "IX_MatchEvents_InvokerID",
                table: "MatchEvents",
                newName: "IX_MatchEvents_InvokerId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchEvents_Matchs_MatchId",
                table: "MatchEvents",
                column: "MatchId",
                principalTable: "Matchs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchEvents_Teams_InvokerId",
                table: "MatchEvents",
                column: "InvokerId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchEvents_Matchs_MatchId",
                table: "MatchEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchEvents_Teams_InvokerId",
                table: "MatchEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "InvokerId",
                table: "MatchEvents",
                newName: "InvokerID");

            migrationBuilder.RenameIndex(
                name: "IX_MatchEvents_InvokerId",
                table: "MatchEvents",
                newName: "IX_MatchEvents_InvokerID");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchEvents_Matchs_MatchId",
                table: "MatchEvents",
                column: "MatchId",
                principalTable: "Matchs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchEvents_Teams_InvokerID",
                table: "MatchEvents",
                column: "InvokerID",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Teams_TeamId",
                table: "Players",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
