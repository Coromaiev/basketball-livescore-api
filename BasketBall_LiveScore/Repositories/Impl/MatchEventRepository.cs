using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Infrastructure;

namespace BasketBall_LiveScore.Repositories.Impl
{
    public class MatchEventRepository : IMatchEventRepository
    {
        private readonly LiveScoreContext Context;

        public MatchEventRepository(LiveScoreContext context)
        {
            Context = context;
        }

        // Base class methods
        public async Task<MatchEvent?> GetById(Guid id) => await Context.MatchEvents.FirstOrDefaultAsync(matchEvent => matchEvent.Id.Equals(id));

        public async Task<MatchEvent> Update(MatchEvent matchEvent, TimeSpan? newTime, byte? newQuarterNumber)
        {
            if (newTime.HasValue)
                matchEvent.Time = newTime.Value;

            if (newQuarterNumber.HasValue)
                matchEvent.QuarterNumber = newQuarterNumber.Value;

            await Context.SaveChangesAsync();
            return matchEvent;
        }

        public async Task Delete(MatchEvent matchEvent)
        {
            Context.MatchEvents.Remove(matchEvent);
            await Context.SaveChangesAsync();
        }

        // Fault specific methods
        public async IAsyncEnumerable<Fault> GetFaultsOfMatch(Guid matchId)
        {
            await foreach (var fault in Context.Faults
                .Where(f => f.MatchId.Equals(matchId))
                .Include(f => f.FaultyPlayer)
                .AsAsyncEnumerable())
            {
                yield return fault;
            }
        }

        public async Task<Fault> CreateFault(Fault fault)
        {
            await Context.Faults.AddAsync(fault);
            await Context.SaveChangesAsync();
            return fault;
        }

        // Player Changes specific methods
        public async IAsyncEnumerable<PlayerChange> GetPlayerChangesOfMatch(Guid matchId)
        {
            await foreach (var playerChange in Context.PlayerChanges
                .Where(pc => pc.MatchId.Equals(matchId))
                .Include(pc => pc.LeavingPlayer)
                .Include(pc => pc.ReplacingPlayer)
                .AsAsyncEnumerable())
            {
                yield return playerChange;
            }
        }

        public async Task<PlayerChange> CreatePlayerChange(PlayerChange playerChange)
        {
            await Context.PlayerChanges.AddAsync(playerChange);
            await Context.SaveChangesAsync();
            return playerChange;
        }

        // Score Changes specific methods
        public async IAsyncEnumerable<ScoreChange> GetScoreChangesOfMatch(Guid matchId)
        {
            await foreach (var scoreChange in Context.ScoreChanges
                .Where(sc => sc.MatchId.Equals(matchId))
                .Include(sc => sc.Scorer)
                .AsAsyncEnumerable())
            {
                yield return scoreChange;
            }
        }

        public async Task<ScoreChange> CreateScoreChange(ScoreChange scoreChange)
        {
            await Context.ScoreChanges.AddAsync(scoreChange);
            await Context.SaveChangesAsync();
            return scoreChange;
        }

        // Timeouts specific methods
        public async IAsyncEnumerable<TimeOut> GetTimeOutsOfMatch(Guid matchId)
        {
            await foreach (var timeOut in Context.TimeOuts
                .Where(t => t.MatchId == matchId)
                .Include(t => t.Invoker)
                .AsAsyncEnumerable())
            {
                yield return timeOut;
            }
        }

        public async Task<TimeOut> CreateTimeOut(TimeOut timeOut)
        {
            await Context.TimeOuts.AddAsync(timeOut);
            await Context.SaveChangesAsync();
            return timeOut;
        }
    }
}

