using BasketBall_LiveScore.Infrastructure;
using BasketBall_LiveScore.Models;
using Microsoft.EntityFrameworkCore;

namespace BasketBall_LiveScore.Repositories.Impl
{
    public class TeamRepository : ITeamRepository
    {
        private readonly LiveScoreContext Context;

        public TeamRepository(LiveScoreContext context)
        {
            Context = context; 
        }

        public async Task<Team> AddPlayer(Team team, Player player)
        {
            team.Players.Add(player);
            await Context.SaveChangesAsync();
            return team;
        }

        public async Task<Team> Create(Team team)
        {
            Context.Teams.Add(team);
            await Context.SaveChangesAsync();
            return team;
        }

        public async Task Delete(Team team)
        {
            Context.Teams.Remove(team);
            await Context.SaveChangesAsync();
        }

        public async IAsyncEnumerable<Team> GetAll()
        {
            var teams = Context.Teams
                .Include(team => team.Players)
                .AsAsyncEnumerable();
            await foreach (var team in teams)
            {
                yield return team;
            }
        }

        public async Task<Team?> GetById(Guid id)
        {
            var team = await Context.Teams
                .Include(team => team.Players)
                .FirstOrDefaultAsync(team => team.Id.Equals(id));
            return team;
        }

        public async Task<Team> RemovePlayer(Team team, Player player)
        {
            team.Players.Remove(player);
            await Context.SaveChangesAsync();
            return team;
        }

        public async Task<Team> Update(Team team, string newName)
        {
            team.Name = newName;
            await Context.SaveChangesAsync();
            return team;
        }
    }
}
