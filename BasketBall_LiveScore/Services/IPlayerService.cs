using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Services
{
    public interface IPlayerService
    {
        public IAsyncEnumerable<PlayerDto>? GetAll();
        public IAsyncEnumerable<PlayerDto>? GetByTeam(Guid teamId);
        public Task<PlayerDto?> GetById(Guid id);
        public Task<PlayerDto?> Create(PlayerCreateDto player);
        public Task<PlayerDto?> Update(Guid id, PlayerUpdateDto updatedPlayer);
        public Task<PlayerDto?> UpdateQuitTeam(Guid id);
        public Task Delete(Guid id);
    }
}
