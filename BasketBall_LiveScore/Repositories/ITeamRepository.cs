using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Repositories
{
    public interface ITeamRepository
    {
        public Task<Team?> GetById(Guid id);
        public IAsyncEnumerable<Team> GetAll();
        public Task<Team> Create(Team team);
        public Task<Team> Update(Team team, string newName);
        public Task<Team> AddPlayer(Team team, Player player);
        public Task<Team> RemovePlayer(Team team, Player player);
        public Task Delete(Team team);
    }
}
