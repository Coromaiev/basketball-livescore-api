using BasketBall_LiveScore.Models;
using System.Numerics;

namespace BasketBall_LiveScore.Mappers.Impl
{
    public class MatchMapper : IMatchMapper
    {
        private readonly ITeamMapper TeamMapper;
        private readonly IUserMapper UserMapper;

        public MatchMapper(ITeamMapper teamMapper, IUserMapper userMapper)
        {
            TeamMapper = teamMapper;
            UserMapper = userMapper;
        }

        public MatchDto ConvertToDto(Match match) => new
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
                                                        match.VisitorsScore
                                                     );

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
        public PlayerDto ConvertToDto(Player player) => new(
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
}
