using BasketBall_LiveScore.Infrastructure;
using BasketBall_LiveScore.Models;
using Microsoft.EntityFrameworkCore;

namespace BasketBall_LiveScore.Repositories.Impl
{
    public class MatchRepository : IMatchRepository
    {
        private readonly LiveScoreContext Context;

        public MatchRepository(LiveScoreContext context)
        {
            Context = context;
        }

        public async Task<Match?> GetById(Guid id)
        {
            var match = await Context.Matchs.FirstOrDefaultAsync(m => m.Id.Equals(id));
            return match;
        }
    }
}
