using BasketBall_LiveScore.Infrastructure;
using BasketBall_LiveScore.Models;
using Microsoft.EntityFrameworkCore;

namespace BasketBall_LiveScore.Repositories.Impl
{
    public class MatchRepository : IMatchRepository
    {
        private readonly LiveScoreContext Context;

        public MatchRepository(LiveScoreContext context)
        {
            Context = context;
        }

        public async Task<Match> AddEvent(Match match, MatchEvent matchEvent)
        {
            match.Events.Add(matchEvent);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> AddHostStartingPlayers(Match match, IEnumerable<Player> players)
        {
            match.HostsStartingPlayers.AddRange(players);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> AddPlayEncoders(Match match, IEnumerable<User> playEncoders)
        {
            match.PlayEncoders.AddRange(playEncoders);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> AddVisitorStartingPlayers(Match match, IEnumerable<Player> players)
        {
            match.VisitorsStartingPlayers.AddRange(players);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> Create(Match match)
        {
            await Context.Matchs.AddAsync(match);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task Delete(Match match)
        {
            Context.Matchs.Remove(match);
            await Context.SaveChangesAsync();
        }

        public async IAsyncEnumerable<Match> GetAll()
        {
            var matchs = Context.Matchs
                .Include(match => match.Visitors)
                    .ThenInclude(team => team.Players)
                .Include(match => match.Hosts)
                    .ThenInclude(team => team.Players)
                .Include(match => match.PrepEncoder)
                .Include(match => match.PlayEncoders)
                .AsAsyncEnumerable();
            await foreach (var match in matchs)
            {
                yield return match;
            }
        }

        public async IAsyncEnumerable<Match> GetByEncoder(User encoder)
        {
            var encoderMatchs = Context.Matchs
                .Where(match => match.PlayEncoders.Any(playEncoder => playEncoder.Id.Equals(encoder.Id))
                                || (match.PrepEncoder != null && match.PrepEncoder.Id.Equals(encoder.Id)))
                .Include(match => match.Visitors)
                    .ThenInclude(team => team.Players)
                .Include(match => match.Hosts)
                    .ThenInclude(team => team.Players)
                .Include(match => match.PrepEncoder)
                .Include(match => match.PlayEncoders)
                .AsAsyncEnumerable();
            await foreach (var encoderMatch in encoderMatchs) { yield return encoderMatch; }
        }

        public async Task<Match?> GetById(Guid id)
        {
            var match = await Context.Matchs
                .Include(match => match.Visitors)
                    .ThenInclude(team => team.Players)
                .Include(match => match.Hosts)
                    .ThenInclude(team => team.Players)
                .Include(match => match.PrepEncoder)
                .Include(match => match.PlayEncoders)
                .FirstOrDefaultAsync(m => m.Id.Equals(id));
            return match;
        }

        public async IAsyncEnumerable<Match> GetByTeam(Team team)
        {
            var teamMatchs = Context.Matchs
                .Where(match => match.Hosts.Id.Equals(team.Id) || match.Visitors.Id.Equals(team.Id))
                .Include(match => match.Visitors)
                    .ThenInclude(team => team.Players)
                .Include(match => match.Hosts)
                    .ThenInclude(team => team.Players)
                .Include(match => match.PrepEncoder)
                .Include(match => match.PlayEncoders)
                .AsAsyncEnumerable();
            await foreach (var match in teamMatchs)
            {
                yield return match;
            }
        }

        public async IAsyncEnumerable<Match> GetFinished(bool isFinished)
        {
            var matchs = Context.Matchs
                .Where(match => match.IsFinished == isFinished)
                .Include(match => match.Visitors)
                    .ThenInclude(team => team.Players)
                .Include(match => match.Hosts)
                    .ThenInclude(team => team.Players)
                .Include(match => match.PrepEncoder)
                .Include(match => match.PlayEncoders)
                .AsAsyncEnumerable();
            await foreach (var match in matchs) { yield return match; }
        }

        public async Task<Match> RemoveEvent(Match match, MatchEvent matchEvent)
        {
            match.Events.Remove(matchEvent);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> RemoveHostStartingPlayers(Match match, IEnumerable<Player> players)
        {
            match.HostsStartingPlayers.RemoveAll(player => players.Contains(player));
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> RemovePlayEncoders(Match match, IEnumerable<User> playEncoders)
        {
            match.PlayEncoders.RemoveAll(playEncoder => playEncoders.Contains(playEncoder));
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> RemoveVisitorStartingPlayers(Match match, IEnumerable<Player> players)
        {
            match.VisitorsStartingPlayers.RemoveAll(player => players.Contains(player));
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> UpdatePlayDetails(Match match, ulong? hostsScore, ulong? visitorsScore, bool? isFinished, Team? winner)
        {
            if (hostsScore.HasValue) match.HostsScore = hostsScore.Value;
            if (visitorsScore.HasValue) match.VisitorsScore = visitorsScore.Value;
            if (isFinished.HasValue) match.IsFinished = isFinished.Value;
            if (winner is not null)
            {
                match.Winner = winner;
                match.WinnerId = winner.Id;
            }
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> UpdatePrepDetails(Match match, Team? hosts, Team? visitors, User? prepEncoder, byte? quarterDuration, byte? numberOfQuarters, byte? timeoutDuration, bool? hasStarted)
        {
            if (quarterDuration.HasValue) match.QuarterDuration = quarterDuration.Value;
            if (numberOfQuarters.HasValue) match.NumberOfQuarters = numberOfQuarters.Value;
            if (timeoutDuration.HasValue) match.TimeOutDuration = timeoutDuration.Value;
            if (hasStarted.HasValue) match.HasStarted = hasStarted.Value;
            if (hosts is not null)
            {
                match.Hosts = hosts;
                match.HostsId = hosts.Id;
            }
            if (visitors is not null)
            {
                match.Visitors = visitors;
                match.VisitorsId = visitors.Id;
            }
            if (prepEncoder is not null)
            {
                match.PrepEncoder = prepEncoder;
                match.PrepEncoderId = prepEncoder.Id;
            }
            await Context.SaveChangesAsync();
            return match;
        }
    }
}
