using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Repositories
{
    public interface IMatchRepository
    {
        public Task<Match?> GetById(Guid id);
        public Task<Match> Create(Match match);
        public Task<Match> UpdatePrepDetails(Match match, Team? hosts, Team? visitors, User? prepEncoder, byte? quarterDuration, byte? numberOfQuarters, byte? timeoutDuration, bool? hasStarted);
        public Task<Match> UpdatePlayDetails(Match match, ulong? hostsScore, ulong? visitorsScore, bool? isFinished, Team? winner);
        public Task<Match> AddPlayEncoders(Match match, IEnumerable<User> playEncoders);
        public Task<Match> RemovePlayEncoders(Match match, IEnumerable<User> playEncoders);
        public Task<Match> AddHostStartingPlayers(Match match, IEnumerable<Player> players);
        public Task<Match> AddVisitorStartingPlayers(Match match, IEnumerable<Player> players);
        public Task<Match> RemoveHostStartingPlayers(Match match, IEnumerable<Player> players);
        public Task<Match> RemoveVisitorStartingPlayers(Match match, IEnumerable<Player> players);
        public Task<Match> AddEvent(Match match, MatchEvent matchEvent);
        public Task<Match> RemoveEvent(Match match, MatchEvent matchEvent);
        public Task Delete(Match match);
        public IAsyncEnumerable<Match> GetAll();
        public IAsyncEnumerable<Match> GetByTeam(Team team);
        public IAsyncEnumerable<Match> GetFinished(bool isFinished);
        public IAsyncEnumerable<Match> GetByEncoder(User encoder);
    }
}
