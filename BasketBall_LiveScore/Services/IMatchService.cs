using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Services
{
    public interface IMatchService
    {
        public Task<MatchDto> GetById(Guid id);
        public Task<MatchDto> Create(MatchCreateDto match);
        public Task<MatchDto> UpdatePrepDetails(Guid id, MatchUpdatePrepDto match);
        public Task<MatchDto> UpdatePlayDetails(Guid id, MatchUpdatePlayDto match);
        public Task<MatchDto> UpdatePlayEncoders(Guid id, MatchUpdateListDto encodersChanges);
        public Task<MatchDto> UpdateHostsStartingPlayers(Guid id, MatchUpdateListDto hostsStartingPlayersChanges);
        public Task<MatchDto> UpdateVisitorsStartingPlayers(Guid id, MatchUpdateListDto visitorsStartingPlayersChanges);
        public Task Delete(Guid id);
        public IAsyncEnumerable<MatchDto> GetAll();
        public IAsyncEnumerable<MatchDto> GetByTeam(Guid teamId);
        public IAsyncEnumerable<MatchDto> GetWithEndStatus(bool endStatus);
        public IAsyncEnumerable<MatchDto> GetByEncoder(Guid id);
    }
}