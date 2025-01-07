using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Repositories
{
    public interface IMatchRepository
    {
        public Task<Match?> GetById(Guid id);
    }
}
