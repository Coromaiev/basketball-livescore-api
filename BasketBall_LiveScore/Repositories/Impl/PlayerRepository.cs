using BasketBall_LiveScore.Infrastructure;
using BasketBall_LiveScore.Models;
using Microsoft.EntityFrameworkCore;

namespace BasketBall_LiveScore.Repositories.Impl
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly LiveScoreContext Context;

        public PlayerRepository(LiveScoreContext context)
        {
            Context = context;
        }

        public async Task<Player?> GetById(Guid id)
        {
            var foundPlayer = await Context.Players
                .Include(player => player.Team)
                .FirstOrDefaultAsync(p => p.Id.Equals(id));
            return foundPlayer;
        }

        public async IAsyncEnumerable<Player?> GetAll()
        {
            var players = Context.Players
                .Include(player => player.Team)
                .AsAsyncEnumerable();
            await foreach (var player in players) 
            {
                yield return player;
            }

        }

        public async IAsyncEnumerable<Player?> GetByTeam(Guid teamId)
        {
            var teamPlayers = Context.Players
                .Where(player => player.TeamId.Equals(teamId))
                .Include(player => player.Team)
                .AsAsyncEnumerable();
            await foreach (var player in teamPlayers)
            {
                yield return player;
            }
        }

        public async Task<Player> Create(Player player)
        {
            await Context.Players.AddAsync(player);
            await Context.SaveChangesAsync();
            return player;
        }

        public async Task<Player?> Update(Player player, byte? newNumber, Team? newTeam, string? newFirstName, string? newLastName)
        {
            Console.WriteLine($"{newTeam.Id}");
            if (newNumber.HasValue) player.Number = newNumber.Value;
            if (newTeam is not null)
            {
                player.Team = newTeam;
                player.TeamId = newTeam.Id;
            }
            if (!string.IsNullOrWhiteSpace(newFirstName)) player.FirstName = newFirstName;
            if (!string.IsNullOrWhiteSpace(newLastName)) player.LastName = newLastName;
            await Context.SaveChangesAsync();
            return player;
        }

        public async Task Delete(Player player)
        {
            Context.Players.Remove(player);
            await Context.SaveChangesAsync();
        }
    }
}
