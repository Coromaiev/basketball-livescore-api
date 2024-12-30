using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Repositories
{
    public interface IPlayerRepository
    {
        public Task<Player?> GetById(Guid id);
        public IAsyncEnumerable<Player?> GetAll();
        public IAsyncEnumerable<Player?> GetByTeam(Guid teamId);
        public Task<Player> Create(Player player);
        public Task<Player> Update(Player player, byte? newNumber, Team? newTeam, string? newFirstName, string? newLastName);
        public Task<Player> RemoveTeam(Player player);
        public Task Delete(Player player);
    }
}
