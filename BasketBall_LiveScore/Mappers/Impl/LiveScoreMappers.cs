using BasketBall_LiveScore.Models;
using System.Numerics;

namespace BasketBall_LiveScore.Mappers.Impl
{
    public class MatchMapper : IMatchMapper
    {
        private readonly ITeamMapper TeamMapper;
        private readonly IUserMapper UserMapper;
        private readonly IMatchEventMapper MatchEventMapper;

        public MatchMapper(ITeamMapper teamMapper, IUserMapper userMapper, IMatchEventMapper matchEventMapper)
        {
            TeamMapper = teamMapper;
            UserMapper = userMapper;
            MatchEventMapper = matchEventMapper;
        }

        public MatchDto ConvertToDto(Match match)
        {
            var matchDto = new MatchDto
            (
                match.Id,
                match.QuarterDuration,
                match.NumberOfQuarters,
                match.TimeOutDuration,
                TeamMapper.ConvertToDto(match.Hosts),
                TeamMapper.ConvertToDto(match.Visitors),
                match.HostsStartingPlayers.Select(player => player.Id)
                                          .Concat(match.VisitorsStartingPlayers.Select(player => player.Id))
                                          .ToList(),
                match.PrepEncoder is not null ? UserMapper.ConvertToDto(match.PrepEncoder) : null,
                match.PlayEncoders.Select(encoder => UserMapper.ConvertToDto(encoder)).ToList(),
                match.HostsScore,
                match.VisitorsScore,
                (
                 Faults: FilterAndConvert<Fault, FaultDto>(match.Events, MatchEventMapper.ConvertToDto),
                 PlayerChanges: FilterAndConvert<PlayerChange, PlayerChangeDto>(match.Events, MatchEventMapper.ConvertToDto),
                 ScoreChanges: FilterAndConvert<ScoreChange, ScoreChangeDto>(match.Events, MatchEventMapper.ConvertToDto),
                 TimeOuts: FilterAndConvert<TimeOut, TimeOutDto>(match.Events, MatchEventMapper.ConvertToDto)
                ),
                match.IsFinished,
                match.HasStarted,
                match.WinnerId
            );
            return matchDto;
        }

        private static List<TDto> FilterAndConvert<TEntity, TDto>(IEnumerable<MatchEvent> matchEvents, Func<TEntity, TDto> mapFunction) where TEntity : MatchEvent
        {
            return matchEvents.OfType<TEntity>()
                              .Select(mapFunction)
                              .ToList();
        }
            

        public Match ConvertToEntity(MatchCreateDto matchDto, Team hosts, Team visitors, User? prepEncoder) => new()
            {
                Hosts = hosts,
                HostsId = hosts.Id,
                Visitors = visitors,
                VisitorsId = visitors.Id,
                PrepEncoder = prepEncoder,
                PrepEncoderId = prepEncoder?.Id,
                QuarterDuration = matchDto.QuarterDuration ?? Match.StandardQuarterDuration,
                NumberOfQuarters = matchDto.QuarterNumber ?? Match.StandardNumberOfQuarters,
                TimeOutDuration = matchDto.TimeoutDuration ?? Match.StandardTimeOutDurationMins,
            };
    }

    public class PlayerMapper : IPlayerMapper
    {
        public PlayerDto ConvertToDto(Player player) => new
            (
                player.Id,
                player.FirstName,
                player.LastName,
                player.TeamId,
                player.Number
            );

        public Player ConvertToEntity(PlayerCreateDto playerDto) => new()
            {
                FirstName = playerDto.FirstName,
                LastName = playerDto.LastName,
                TeamId = playerDto.TeamId,
                Number = playerDto.Number,
            };
    }      

    public class TeamMapper : ITeamMapper
    {
        private readonly IPlayerMapper PlayerMapper;

        public TeamMapper(IPlayerMapper playerMapper) {  PlayerMapper = playerMapper; }

        public TeamDto ConvertToDto(Team team) => new
            (
                team.Id,
                team.Name,
                team.Players.Select(player => PlayerMapper.ConvertToDto(player)).ToList()
            );

        public Team ConvertToEntity(string name) => new()
            {
                Name = name,
            };
    }

    public class UserMapper : IUserMapper
    {
        public UserDto ConvertToDto(User user) => new(user.Id, user.Username, user.Email, user.Permission);

        public User ConvertToEntity(UserCreateDto userDto) => new()
            {
                Email = userDto.Email,
                Password = userDto.Password,
                Username = userDto.Username,
                Permission = userDto.Permission,
            };
    }

    public class MatchEventMapper : IMatchEventMapper
    {
        public FaultDto ConvertToDto(Fault fault) => new
            (
                fault.Id,
                fault.Time,
                fault.QuarterNumber,
                fault.MatchId,
                fault.FaultyPlayerId,
                fault.Type
            );

        public PlayerChangeDto ConvertToDto(PlayerChange playerChange) => new
            (
                playerChange.Id,
                playerChange.Time,
                playerChange.QuarterNumber,
                playerChange.MatchId,
                playerChange.LeavingPlayerId,
                playerChange.ReplacingPlayerId
            );

        public ScoreChangeDto ConvertToDto(ScoreChange scoreChange) => new
            (
                scoreChange.Id,
                scoreChange.Time,
                scoreChange.QuarterNumber,
                scoreChange.MatchId,
                scoreChange.ScorerId,
                scoreChange.Score
            );

        public TimeOutDto ConvertToDto(TimeOut timeOut) => new
            (
                timeOut.Id,
                timeOut.Time,
                timeOut.QuarterNumber,
                timeOut.MatchId,
                timeOut.InvokerId
            );

        public Fault ConvertToEntity(FaultCreateDto faultDto, Match match, Player faultyPlayer)
        {
            Fault fault = new()
            {
                FaultyPlayer = faultyPlayer,
                FaultyPlayerId = faultyPlayer.Id,
                Type = faultDto.FaultType
            };
            SetBaseEntityValues(faultDto, fault, match);
            Console.WriteLine($"{fault.Time}, {fault.QuarterNumber}");
            return fault;
        }

        public PlayerChange ConvertToEntity(PlayerChangeCreateDto playerChangeDto, Match match, Player leavingPlayer, Player replacingPlayer)
        {
            PlayerChange playerChange = new()
            {
                LeavingPlayer = leavingPlayer,
                LeavingPlayerId = leavingPlayer.Id,
                ReplacingPlayer = replacingPlayer,
                ReplacingPlayerId = replacingPlayer.Id
            };
            SetBaseEntityValues(playerChangeDto, playerChange, match);
            return playerChange;
        }

        public ScoreChange ConvertToEntity(ScoreChangeCreateDto scoreChangeDto, Match match, Player scorer)
        {
            ScoreChange scoreChange = new()
            {
                Scorer = scorer,
                ScorerId = scorer.Id,
                Score = scoreChangeDto.Points,
            };
            SetBaseEntityValues(scoreChangeDto, scoreChange, match);
            return scoreChange;
        }

        public TimeOut ConvertToEntity(TimeOutCreateDto timeOutDto, Match match, Team invoker)
        {
            TimeOut timeOut = new()
            {
                Invoker = invoker,
                InvokerId = invoker.Id,
            };
            SetBaseEntityValues(timeOutDto, timeOut, match);
            return timeOut;
        }

        private static void SetBaseEntityValues(MatchEventCreateDto matchEventDto, MatchEvent matchEvent, Match match)
        {
            matchEvent.Match = match;
            matchEvent.MatchId = match.Id;
            matchEvent.Time = matchEventDto.Time;
            matchEvent.QuarterNumber = matchEventDto.QuarterNumber;
        }
    }
}
