using BasketBall_LiveScore.Infrastructure;
using BasketBall_LiveScore.Models;
using Microsoft.EntityFrameworkCore;

namespace BasketBall_LiveScore.Repositories.Impl
{
    public class PlayerRepository
    {
        private readonly LiveScoreContext Context;

        public PlayerRepository(LiveScoreContext context)
        {
            Context = context;
        }

        public async Task<Player?> GetById(ulong id)
        {
            var foundPlayer = await Context.Players.FirstOrDefaultAsync(p => p.Id.Equals(id));
            return foundPlayer;
        }

        public async IAsyncEnumerable<Player>? GetAll()
        {
            foreach (var player in await Context.Players.ToListAsync()) 
            {
                yield return player;
            }

        }
    }
}
