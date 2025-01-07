using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Repositories;

namespace BasketBall_LiveScore.Services.Impl
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository MatchRepository;

        public MatchService(IMatchRepository matchRepository)
        {
            this.MatchRepository = matchRepository;
        }

        public async Task<MatchDto?> GetById(Guid id)
        {
            var match = await MatchRepository.GetById(id);
            if (match is null)
            {
                return null;
            }
            return ConvertToDto(match);
        }

        private MatchDto ConvertToDto(Match match)
        {
            var matchDto = new MatchDto
                (
                    match.Id,
                    match.QuarterDuration,
                    match.NumberOfQuarters,
                    match.TimeOutDuration,
                    (Guid)match.VisitorsId,
                    (Guid)match.HostsId,
                    (Guid)match.PrepEncoderId,
                    (Guid)match.PlayEncoderId,
                    match.HostsScore,
                    (ulong)match.VisitorsScore
                );
            return matchDto;
        }
    }
}
