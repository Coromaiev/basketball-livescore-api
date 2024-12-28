using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Services
{
    public interface IMatchService
    {
        public Task<MatchDto?> GetById(Guid id);
    }
}
