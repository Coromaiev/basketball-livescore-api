using BasketBall_LiveScore.Models;

namespace BasketBall_LiveScore.Repositories
{
    public interface IMatchRepository
    {
        public Task<Match?> GetById(Guid id);
        public Task<Match> Create(Match match);
        public Task<Match> UpdatePrepDetails(Match match, Team? hosts, Team? visitors, User? prepEncoder, byte? quarterDuration, byte? numberOfQuarters, byte? timeoutDuration, bool? hasStarted);
        public Task<Match> UpdatePlayDetails(Match match, ulong? hostsScore, ulong? visitorsScore, bool? isFinished, Team? winner);
        public Task<Match> AddPlayEncoder(Match match, User playEncoder);
        public Task<Match> RemovePlayEncoder(Match match, User playEncoder);
        public Task<Match> AddHostStartingPlayer(Match match, Player player);
        public Task<Match> AddVisitorStartingPlayer(Match match, Player player);
        public Task<Match> RemoveHostStartingPlayer(Match match, Player player);
        public Task<Match> RemoveVisitorStartingPlayer(Match match, Player player);
        public Task<Match> AddEvent(Match match, MatchEvent matchEvent);
        public Task<Match> RemoveEvent(Match match, MatchEvent matchEvent);
        public IAsyncEnumerable<Match> GetAll();
        public IAsyncEnumerable<Match> GetByTeam(Team team);
        public IAsyncEnumerable<Match> GetFinished(bool isFinished);
        public IAsyncEnumerable<Match> GetByEncoder(User encoder);
    }
}
