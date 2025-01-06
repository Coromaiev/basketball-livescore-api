using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Repositories
{
    public interface IMatchEventRepository
    {
        // Base class methods
        public Task<MatchEvent?> GetById(Guid id);
        public Task<MatchEvent> Update(MatchEvent matchEvent, TimeSpan? newTime, byte? newQuarterNumber);
        public Task Delete(MatchEvent matchEvent);

        // Fault specific methods
        public IAsyncEnumerable<Fault> GetFaultsOfMatch(Guid matchId);
        public Task<Fault> CreateFault(Fault fault);

        // Player Changes specific methods
        public IAsyncEnumerable<PlayerChange> GetPlayerChangesOfMatch(Guid matchId);
        public Task<PlayerChange> CreatePlayerChange(PlayerChange playerChange);

        // Score Changes specific methods
        public IAsyncEnumerable<ScoreChange> GetScoreChangesOfMatch(Guid matchId);
        public Task<ScoreChange> CreateScoreChange(ScoreChange scoreChange);

        // Timeouts specific methods
        public IAsyncEnumerable<TimeOut> GetTimeOutsOfMatch(Guid matchId);
        public Task<TimeOut> CreateTimeOut(TimeOut timeOut);
    }
}
