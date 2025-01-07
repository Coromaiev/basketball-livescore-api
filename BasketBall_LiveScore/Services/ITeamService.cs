using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Services
{
    public interface ITeamService
    {
        public IAsyncEnumerable<TeamDto> GetAll();
        public Task<TeamDto> GetById(Guid id);
        public Task<TeamDto> Create(string name);
        public Task<TeamDto> Update(Guid id, string newName);
        public Task Delete(Guid id);
    }
}
