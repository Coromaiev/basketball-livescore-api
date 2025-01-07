using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Services
{
    public interface IMatchEventService
    {
        public Task<(IAsyncEnumerable<FaultDto> Faults, IAsyncEnumerable<PlayerChangeDto> PlayerChanges, IAsyncEnumerable<ScoreChangeDto> ScoreChanges, IAsyncEnumerable<TimeOutDto> TimeOuts)> GetAllEventsByMatch(Guid id);

        public Task DeleteEvent(Guid eventId);
        public Task UpdateEvent(Guid eventId, MatchEventUpdateDto updateDto);
        public IAsyncEnumerable<TDto> GetEventsByMatch<T, TDto>(Guid matchId)
            where T : MatchEvent
            where TDto : MatchEventDto;
        public Task<MatchEventDto> GetById(Guid id);
        public Task<FaultDto> CreateFault(FaultCreateDto faultDto);
        public Task<PlayerChangeDto> CreatePlayerChange(PlayerChangeCreateDto playerChangeDto);
        public Task<ScoreChangeDto> CreateScoreChange(ScoreChangeCreateDto scoreChangeDto);
        public Task<TimeOutDto> CreateTimeOut(TimeOutCreateDto timeOutDto);
    }
}
