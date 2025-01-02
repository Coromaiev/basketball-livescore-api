using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Mappers
{
    public interface IMatchMapper
    {
        public MatchDto ConvertToDto(Match match);
        public Match ConvertToEntity(MatchCreateDto matchDto, Team hosts, Team visitors, User? prepEncoder);
    }

    public interface IPlayerMapper
    {
        public PlayerDto ConvertToDto(Player player);
        public Player ConvertToEntity(PlayerCreateDto playerDto);
    }

    public interface ITeamMapper
    {
        public TeamDto ConvertToDto(Team team);
        public Team ConvertToEntity(string name);
    }

    public interface IUserMapper
    {
        public UserDto ConvertToDto(User user);
        public User ConvertToEntity(UserCreateDto userDto);
    }

}
