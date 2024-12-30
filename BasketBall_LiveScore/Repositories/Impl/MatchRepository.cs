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

        public async Task<Match> AddHostStartingPlayer(Match match, Player player)
        {
            match.HostsStartingPlayers.Add(player);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> AddPlayEncoder(Match match, User playEncoder)
        {
            match.PlayEncoders.Add(playEncoder);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> AddVisitorStartingPlayer(Match match, Player player)
        {
            match.VisitorsStartingPlayers.Add(player);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> Create(Match match)
        {
            await Context.Matchs.AddAsync(match);
            await Context.SaveChangesAsync();
            return match;
        }

        public async IAsyncEnumerable<Match> GetAll()
        {
            var matchs = Context.Matchs;
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
                .AsAsyncEnumerable();
            await foreach (var encoderMatch in encoderMatchs) { yield return encoderMatch; }
        }

        public async Task<Match?> GetById(Guid id)
        {
            var match = await Context.Matchs.FirstOrDefaultAsync(m => m.Id.Equals(id));
            return match;
        }

        public async IAsyncEnumerable<Match> GetByTeam(Team team)
        {
            var teamMatchs = Context.Matchs.Where(match => match.Hosts.Id.Equals(team.Id) || match.Visitors.Id.Equals(team.Id)).AsAsyncEnumerable();
            await foreach (var match in teamMatchs)
            {
                yield return match;
            }
        }

        public async IAsyncEnumerable<Match> GetFinished(bool isFinished)
        {
            var matchs = Context.Matchs.Where(match => match.IsFinished == isFinished).AsAsyncEnumerable();
            await foreach (var match in matchs) { yield return match; }
        }

        public async Task<Match> RemoveEvent(Match match, MatchEvent matchEvent)
        {
            match.Events.Remove(matchEvent);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> RemoveHostStartingPlayer(Match match, Player player)
        {
            match.HostsStartingPlayers.Remove(player);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> RemovePlayEncoder(Match match, User playEncoder)
        {
            match.PlayEncoders.Remove(playEncoder);
            await Context.SaveChangesAsync();
            return match;
        }

        public async Task<Match> RemoveVisitorStartingPlayer(Match match, Player player)
        {
            match.VisitorsStartingPlayers.Remove(player);
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
