using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Repositories
{
    public interface IPlayerRepository
    {
        public Player GetById(ulong id);
    }
}
