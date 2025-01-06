using BasketBall_LiveScore.Exceptions;
using BasketBall_LiveScore.Mappers;
using BasketBall_LiveScore.Mappers.Impl;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Repositories;

namespace BasketBall_LiveScore.Services.Impl
{
    public class MatchEventService : IMatchEventService
    {
        private readonly IMatchEventRepository MatchEventRepository;
        private readonly IMatchRepository MatchRepository;
        private readonly IPlayerRepository PlayerRepository;
        private readonly ITeamRepository TeamRepository;
        private readonly IMatchEventMapper Mapper;

        public MatchEventService(IMatchEventRepository matchEventRepository, IMatchRepository matchRepository, IPlayerRepository playerRepository, ITeamRepository teamRepository, IMatchEventMapper mapper)
        {
            MatchEventRepository = matchEventRepository;
            MatchRepository = matchRepository;
            PlayerRepository = playerRepository;
            TeamRepository = teamRepository;
            Mapper = mapper;
        }

        public async Task<FaultDto> CreateFault(FaultCreateDto faultDto)
        {
            ValidateInput(faultDto);
            var match = await ValidateMatch(faultDto.MatchId);
            // Check for time and quarter number limits
            if (faultDto.QuarterNumber > match.NumberOfQuarters && !match.HostsScore.Equals(match.VisitorsScore)) throw new BadRequestException($"This match does not have more than {match.NumberOfQuarters} and is not in prolongation phases");
            if (faultDto.Time > TimeSpan.FromMinutes(match.NumberOfQuarters * match.QuarterDuration) && !match.HostsScore.Equals(match.VisitorsScore)) throw new BadRequestException($"Time {faultDto.Time} is not valid as the match is not in prolongation phases");
            var faultyPlayer = await FindPlayer(faultDto.FaultyPlayerId);
            if (!match.HostsStartingPlayers.Contains(faultyPlayer) && !match.VisitorsStartingPlayers.Contains(faultyPlayer))
                throw new BadRequestException($"{faultyPlayer.FirstName} {faultyPlayer.LastName} is not on the field and thus cannot commit faults");

            var fault = Mapper.ConvertToEntity(faultDto, match, faultyPlayer);
            fault = await MatchEventRepository.CreateFault(fault);
            match = await MatchRepository.AddEvent(match, fault);

            return Mapper.ConvertToDto(fault);

        }

        public async Task<PlayerChangeDto> CreatePlayerChange(PlayerChangeCreateDto playerChangeDto)
        {
            ValidateInput(playerChangeDto);
            var match = await ValidateMatch(playerChangeDto.MatchId);
            var leavingPlayer = await FindPlayer(playerChangeDto.LeavingPlayerId);
            var replacingPlayer = await FindPlayer(playerChangeDto.ReplacingPlayerId);
            if (!leavingPlayer.TeamId.Equals(replacingPlayer.TeamId)) throw new BadRequestException("A player cannot replace or be replaced by a player from another team");
            if (!match.HostsStartingPlayers.Contains(leavingPlayer) && !match.VisitorsStartingPlayers.Contains(leavingPlayer)) throw new BadRequestException("A player who's not on the playing ground cannot be replaced");
            if (match.HostsStartingPlayers.Contains(replacingPlayer) || match.VisitorsStartingPlayers.Contains(replacingPlayer)) throw new BadRequestException("A player who's already on field cannot replace any other one");

            var playerChange = Mapper.ConvertToEntity(playerChangeDto, match, leavingPlayer, replacingPlayer);
            playerChange = await MatchEventRepository.CreatePlayerChange(playerChange);
            if (leavingPlayer.TeamId.Equals(match.HostsId))
            {
                await MatchRepository.RemoveHostStartingPlayers(match, [leavingPlayer]);
                await MatchRepository.AddHostStartingPlayers(match, [replacingPlayer]);
            }
            else
            {
                await MatchRepository.RemoveVisitorStartingPlayers(match, [leavingPlayer]);
                await MatchRepository.AddVisitorStartingPlayers(match, [replacingPlayer]);
            }
            await MatchRepository.AddEvent(match, playerChange);
            return Mapper.ConvertToDto(playerChange);
        }

        public async Task<ScoreChangeDto> CreateScoreChange(ScoreChangeCreateDto scoreChangeDto)
        {
            ValidateInput(scoreChangeDto);
            var match = await ValidateMatch(scoreChangeDto.MatchId);
            var scorer = await FindPlayer(scoreChangeDto.ScorerId);
            if (!match.HostsStartingPlayers.Contains(scorer) && !match.VisitorsStartingPlayers.Contains(scorer))
                throw new BadRequestException($"Players who are not on the field cannot score");
            var scoreChange = Mapper.ConvertToEntity(scoreChangeDto, match, scorer);
            scoreChange = await MatchEventRepository.CreateScoreChange(scoreChange);
            await MatchRepository.UpdatePlayDetails(match, scorer.TeamId.Equals(match.HostsId) ? match.HostsScore + (ulong)scoreChangeDto.Points : null, scorer.TeamId.Equals(match.VisitorsId) ? match.VisitorsScore + (ulong)scoreChangeDto.Points : null, null, null);
            await MatchRepository.AddEvent(match, scoreChange);
            return Mapper.ConvertToDto(scoreChange);
        }

        public async Task<TimeOutDto> CreateTimeOut(TimeOutCreateDto timeOutDto)
        {
            ValidateInput(timeOutDto);
            var match = await ValidateMatch(timeOutDto.MatchId);
            var team = await TeamRepository.GetById(timeOutDto.InvokerId) ?? throw new NotFoundException($"Team with id {timeOutDto.InvokerId} not found");
            if (!team.Id.Equals(match.HostsId) || !team.Id.Equals(match.VisitorsId)) throw new BadRequestException($"Team {team.Name} is not involved in this match");
            var timeOut = Mapper.ConvertToEntity(timeOutDto, match, team);
            timeOut = await MatchEventRepository.CreateTimeOut(timeOut);
            await MatchRepository.AddEvent(match, timeOut);
            return Mapper.ConvertToDto(timeOut);

        }

        public async Task DeleteEvent(Guid eventId)
        {
            var matchEvent = await MatchEventRepository.GetById(eventId) ?? throw new ConflictException($"Event {eventId} not found or already deleted");
            await MatchRepository.RemoveEvent(matchEvent.Match, matchEvent);
            await MatchEventRepository.Delete(matchEvent);
        }

        public async Task<(IAsyncEnumerable<FaultDto> Faults, IAsyncEnumerable<PlayerChangeDto> PlayerChanges, IAsyncEnumerable<ScoreChangeDto> ScoreChanges, IAsyncEnumerable<TimeOutDto> TimeOuts)> GetAllEventsByMatch(Guid matchId)
        {
            await ValidateMatch(matchId);

            async IAsyncEnumerable<FaultDto> Faults()
            {
                await foreach (var fault in MatchEventRepository.GetFaultsOfMatch(matchId))
                {
                    yield return Mapper.ConvertToDto(fault);
                }
            }

            async IAsyncEnumerable<PlayerChangeDto> PlayerChanges()
            {
                await foreach (var playerChange in MatchEventRepository.GetPlayerChangesOfMatch(matchId))
                {
                    yield return Mapper.ConvertToDto(playerChange);
                }
            }

            async IAsyncEnumerable<ScoreChangeDto> ScoreChanges()
            {
                await foreach (var scoreChange in MatchEventRepository.GetScoreChangesOfMatch(matchId))
                {
                    yield return Mapper.ConvertToDto(scoreChange);
                }
            }

            async IAsyncEnumerable<TimeOutDto> TimeOuts()
            {
                await foreach (var timeOut in MatchEventRepository.GetTimeOutsOfMatch(matchId))
                {
                    yield return Mapper.ConvertToDto(timeOut);
                }
            }

            // Return the tuple of async enumerables
            return (Faults: Faults(), PlayerChanges: PlayerChanges(), ScoreChanges: ScoreChanges(), TimeOuts: TimeOuts());
        }

        public async Task<MatchEventDto> GetById(Guid id)
        {
            var matchEvent = await MatchEventRepository.GetById(id) ?? throw new NotFoundException($"Could not find a match event with id {id}");
            return matchEvent switch
            {
                Fault fault => Mapper.ConvertToDto(fault),
                PlayerChange playerChange => Mapper.ConvertToDto(playerChange),
                ScoreChange scoreChange => Mapper.ConvertToDto(scoreChange),
                TimeOut timeOut => Mapper.ConvertToDto(timeOut),
                _ => throw new BadRequestException($"Unrecognized match Event type : {matchEvent.GetType().Name}")
            };
        }

        public async IAsyncEnumerable<TDto> GetEventsByMatch<T, TDto>(Guid matchId)
            where T : MatchEvent
            where TDto : MatchEventDto
        {
            var match = await MatchRepository.GetById(matchId) ?? throw new NotFoundException($"Match with id {matchId} not found");
            var events = match.Events.OfType<T>();
            foreach (var matchEvent in events)
            {
                yield return matchEvent switch
                {
                    Fault fault => (TDto)(object)Mapper.ConvertToDto(fault),
                    PlayerChange playerChange => (TDto)(object)Mapper.ConvertToDto(playerChange),
                    ScoreChange scoreChange => (TDto)(object)Mapper.ConvertToDto(scoreChange),
                    TimeOut timeOut => (TDto)(object)Mapper.ConvertToDto(timeOut),
                    _ => throw new BadRequestException($"Unrecognized match event type : {matchEvent.GetType().Name}")
                };
            }

        }

        private async Task<Player> FindPlayer(Guid playerId)
        {
            var player = await PlayerRepository.GetById(playerId) ?? throw new NotFoundException($"Player with id {playerId} not found");
            if (!player.TeamId.HasValue) throw new BadRequestException("Cannot invoke match events involving lone players");
            return player;
        }

        private async Task<Match> ValidateMatch(Guid matchId)
        {
            var match = await MatchRepository.GetById(matchId) ?? throw new NotFoundException($"Match with id {matchId} not found");
            if (!match.HasStarted || match.IsFinished) throw new UnauthorizedException("Cannot add events to an unstarted or finished match");
            return match;
        }

        private static void ValidateInput(MatchEventCreateDto input)
        {
            if (input.QuarterNumber <= 0) throw new BadRequestException("Cannot provide negative quarter number values");
            if (input.Time <= TimeSpan.Zero) throw new BadRequestException("Cannot provide negative or nullified time values");
        }

        public async Task UpdateEvent(Guid eventId, MatchEventUpdateDto updateDto)
        {
            if (updateDto is null || !(updateDto.QuarterNumber.HasValue || updateDto.Time.HasValue)) throw new BadRequestException("No update data provided");
            var matchEvent = await MatchEventRepository.GetById(eventId) ?? throw new NotFoundException($"Event with id {eventId} not found");
            var match = matchEvent.Match;
            if (updateDto.QuarterNumber.HasValue)
            {
                var quarterNumber = updateDto.QuarterNumber.Value;
                if (quarterNumber > matchEvent.Match.NumberOfQuarters && !match.HostsScore.Equals(match.VisitorsScore)) throw new BadRequestException($"Cannot set quarter number of an event above the match limit of {matchEvent.Match.NumberOfQuarters} outside prolongation phases");
            }
            if (updateDto.Time.HasValue)
            {
                var time = updateDto.Time.Value;
                if (time > TimeSpan.FromMinutes(match.NumberOfQuarters * match.QuarterDuration) && !match.HostsScore.Equals(match.VisitorsScore)) throw new BadRequestException($"Cannot set time above the time limit of {match.QuarterDuration * match.NumberOfQuarters} minutes outside prolongation phases");
            }
            await MatchEventRepository.Update(matchEvent, updateDto.Time, updateDto.QuarterNumber);
        }
    }
}
