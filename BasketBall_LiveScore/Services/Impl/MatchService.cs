using BasketBall_LiveScore.Exceptions;
using BasketBall_LiveScore.Mappers;
using BasketBall_LiveScore.Mappers.Impl;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Repositories;

namespace BasketBall_LiveScore.Services.Impl
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository MatchRepository;
        private readonly ITeamRepository TeamRepository;
        private readonly IPlayerRepository PlayerRepository;
        private readonly IUserRepository UserRepository;
        private readonly IMatchMapper MatchMapper;

        public MatchService(IMatchRepository matchRepository, ITeamRepository teamRepository, IUserRepository userRepository, IMatchMapper matchMapper, IPlayerRepository playerRepository)
        {
            MatchRepository = matchRepository;
            PlayerRepository = playerRepository;
            TeamRepository = teamRepository;
            UserRepository = userRepository;
            MatchMapper = matchMapper;
        }

        public async Task<MatchDto> Create(MatchCreateDto match)
        {
            var hosts = await TeamRepository.GetById(match.HostsId) ?? throw new NotFoundException($"Team {match.HostsId} not found. Impossible assignation to hosts");
            var visitors = await TeamRepository.GetById(match.VisitorsId) ?? throw new NotFoundException($"Team {match.HostsId} not found. Impossible assignation to visitors");
            User? prepEncoder = null;
            if (match.PrepEncoderId.HasValue)
            {
                prepEncoder = await UserRepository.GetById(match.PrepEncoderId.Value) ?? throw new NotFoundException($"User {match.PrepEncoderId} not found");
                if (prepEncoder.Permission < Role.Encoder) throw new UnauthorizedException($"{prepEncoder.Username} does not have enough rights to be assigned as encoder for a match");

            }
            var newMatch = MatchMapper.ConvertToEntity(match, hosts, visitors, prepEncoder);
            newMatch = await MatchRepository.Create(newMatch);
            return MatchMapper.ConvertToDto(newMatch);
        }

        public async Task Delete(Guid id)
        {
            var match = await MatchRepository.GetById(id) ?? throw new ConflictException($"Match {id} has already been deleted or does not exist");
            await MatchRepository.Delete(match);
        }

        public async IAsyncEnumerable<MatchDto> GetAll()
        {
            var matchs = MatchRepository.GetAll() ?? throw new NotFoundException("No matchs currently available");
            await foreach (var match in matchs)
            {
                Console.WriteLine(match.Id);
                Console.WriteLine(match.Hosts.Id);
                yield return MatchMapper.ConvertToDto(match);
            }
        }

        public async IAsyncEnumerable<MatchDto> GetByEncoder(Guid id)
        {
            var encoder = await UserRepository.GetById(id) ?? throw new NotFoundException($"User {id} not found");
            if (encoder.Permission < Role.Encoder) throw new UnauthorizedException($"{encoder.Username} is not an encoder or more privileged user");
            var encoderMatchs = MatchRepository.GetByEncoder(encoder) ?? throw new NotFoundException($"{encoder.Username} does not have any assigned match yet");
            await foreach (var encoderMatch in encoderMatchs)
            {
                yield return MatchMapper.ConvertToDto(encoderMatch);
            }
        }

        public async Task<MatchDto> GetById(Guid id)
        {
            var match = await MatchRepository.GetById(id) ?? throw new NotFoundException($"Match {id} not found");
            return MatchMapper.ConvertToDto(match);
        }

        public async IAsyncEnumerable<MatchDto> GetByTeam(Guid teamId)
        {
            var team = await TeamRepository.GetById(teamId) ?? throw new NotFoundException($"Team {teamId} not found");
            var teamMatchs = MatchRepository.GetByTeam(team) ?? throw new NotFoundException($"{team.Name} has not participated in any match yet");
            await foreach(var match in teamMatchs)
            {
                yield return MatchMapper.ConvertToDto(match);
            }
        }

        public async IAsyncEnumerable<MatchDto> GetWithEndStatus(bool endStatus)
        {
            var matchs = MatchRepository.GetFinished(endStatus) ?? throw new NotFoundException($"No {(endStatus ? "" : "un")}finished match found");
            await foreach (var match in matchs)
            {
                yield return MatchMapper.ConvertToDto(match);
            }
        }

        public async Task<MatchDto> UpdateHostsStartingPlayers(Guid id, MatchUpdateListDto startingPlayersChangesDto)
        {
            return await UpdateTeamStartingPlayers(id, startingPlayersChangesDto, true);
        }

        public async Task<MatchDto> UpdatePlayDetails(Guid id, MatchUpdatePlayDto matchDto)
        {
            var match = await MatchRepository.GetById(id) ?? throw new NotFoundException($"Match with id {id} not found");
            if (matchDto.IsFinished.HasValue != matchDto.WinnerId.HasValue) throw new BadRequestException("A winner cannot be declared if the match is not finished. Cannot finish a match without declaring a winner");
            Team? winner = matchDto.WinnerId.HasValue ? (await TeamRepository.GetById(matchDto.WinnerId.Value) ?? throw new NotFoundException($"Could not find team {matchDto.WinnerId.Value}")): null;
            if (!match.HasStarted) throw new UnauthorizedException("Cannot update playing data from a match which has not started yet");
            if (matchDto.HostsScore < 0 || matchDto.VisitorsScore < 0) throw new BadRequestException("Scores cannot go under 0");

            match = await MatchRepository.UpdatePlayDetails(match, matchDto.HostsScore, matchDto.VisitorsScore, matchDto.IsFinished, winner);
            return MatchMapper.ConvertToDto(match);
        }

        public async Task<MatchDto> UpdatePlayEncoders(Guid id, MatchUpdateListDto playEncodersChangeDto)
        {
            var newEncoders = playEncodersChangeDto.IncomingEntities;
            var removedEncoders = playEncodersChangeDto.LeavingEntities;
            // Fetching the match and validating its status (match started -> update refused)
            var match = await MatchRepository.GetById(id) ?? throw new NotFoundException($"Match {id} not found");
            if (match.HasStarted) throw new UnauthorizedException("Cannot assign encoders to an already started match");
            if (removedEncoders.Any(encoder => !match.PlayEncoders.Select(matchEncoder => matchEncoder.Id).Contains(encoder)))
                throw new BadRequestException("Trying to remove encoders who are not assigned to the match");

            ValidateRequestIds(newEncoders, removedEncoders,
                "An encoder cannot be added nor removed multiple times in the same request",
                "Cannot add and remove the same encoder in one request");

            var encodersDictionary = await FetchAndValidateCollections<User>(newEncoders, removedEncoders, UserRepository.GetById,
                encoder => encoder.Permission >= Role.Encoder || !newEncoders.Contains(encoder.Id),
                "User {0} not found",
                "{0} does not have enough permissions to be assigned as an encoder for this match");

            var leavingEncoders = removedEncoders.Select(encoderId => encodersDictionary[encoderId]);
            var incomingPlayers = newEncoders.Select(encoderId => encodersDictionary[encoderId]);
            match = await MatchRepository.RemovePlayEncoders(match, leavingEncoders);
            match = await MatchRepository.AddPlayEncoders(match, incomingPlayers);
            return MatchMapper.ConvertToDto(match);
        }

        public async Task<MatchDto> UpdatePrepDetails(Guid id, MatchUpdatePrepDto matchDto)
        {
            if (matchDto is null || (!matchDto.TimeoutDuration.HasValue && !matchDto.QuarterDuration.HasValue && !matchDto.QuarterNumber.HasValue && !matchDto.HostsId.HasValue && !matchDto.VisitorsId.HasValue && !matchDto.HasStarted.HasValue && !matchDto.PrepEncoderId.HasValue))
                throw new BadRequestException("No data provided");
            var match = await MatchRepository.GetById(id) ?? throw new NotFoundException($"Match {id} not found");
            if (match.HasStarted) throw new UnauthorizedException("Cannot update playing match");
            if ((matchDto.HostsId.HasValue && matchDto.VisitorsId.HasValue && matchDto.HostsId.Value.Equals(matchDto.VisitorsId.Value))
                || (matchDto.HostsId.HasValue && matchDto.HostsId.Value.Equals(match.VisitorsId))
                || (matchDto.VisitorsId.HasValue && matchDto.VisitorsId.Value.Equals(match.HostsId))) throw new BadRequestException("Cannot assign the same team to both visitors and hosts");
            if (matchDto.TimeoutDuration <= 0 || matchDto.QuarterDuration <= 0 || matchDto.QuarterNumber <= 0) throw new BadRequestException("Cannot assign negative values to match preparation parameters");

            User? prepEncoder = matchDto.PrepEncoderId.HasValue ? (await UserRepository.GetById(matchDto.PrepEncoderId.Value) ?? throw new NotFoundException($"Could not find user {matchDto.PrepEncoderId.Value}")) : null;
            if (prepEncoder is not null && prepEncoder.Permission < Role.Encoder) throw new UnauthorizedException($"{prepEncoder.Username} does not have enough permissions to be assigned as an encoder");
            Team? hosts = matchDto.HostsId.HasValue ? (await TeamRepository.GetById(matchDto.HostsId.Value) ?? throw new NotFoundException($"Team {matchDto.HostsId.Value} not found")) : null;
            Team? visitors = matchDto.VisitorsId.HasValue ? (await TeamRepository.GetById(matchDto.VisitorsId.Value) ?? throw new NotFoundException($"Team {matchDto.VisitorsId.Value} not found")) : null; ;

            match = await MatchRepository.UpdatePrepDetails(match, hosts, visitors, prepEncoder, matchDto.QuarterDuration, matchDto.QuarterNumber, matchDto.TimeoutDuration, matchDto.HasStarted);
            return MatchMapper.ConvertToDto(match);
        }

        public async Task<MatchDto> UpdateVisitorsStartingPlayers(Guid id, MatchUpdateListDto startingPlayersChangesDto)
        {
            return await UpdateTeamStartingPlayers(id, startingPlayersChangesDto, false);
        }

        private static async Task<Dictionary<Guid, T>> FetchAndValidateCollections<T>(IEnumerable<Guid> newEntities, IEnumerable<Guid> removedEntities, Func<Guid, Task<T?>> fetchFunc, Func<T, bool> validationFunc, string notFoundMessage, string invalidEntityMessage)
        {
            var allEntityIds = newEntities.Concat(removedEntities).Distinct();
            var entityDictionary = new Dictionary<Guid, T>();

            foreach (var entityId in allEntityIds)
            {
                if (!entityDictionary.ContainsKey(entityId))
                {
                    var entity = await fetchFunc(entityId) ?? throw new NotFoundException(string.Format(notFoundMessage, entityId));
                    if (!validationFunc(entity)) throw new BadRequestException(string.Format(invalidEntityMessage, entityId));

                    entityDictionary[entityId] = entity;

                }
            }
            return entityDictionary;
        }

        private async Task<MatchDto> UpdateTeamStartingPlayers(Guid id, MatchUpdateListDto startingPlayersChangesDto, bool isHostTeam)
        {
            var newStartingPlayers = startingPlayersChangesDto.IncomingEntities;
            var removedStartingPlayers = startingPlayersChangesDto.LeavingEntities;
            // Fetching the match and validating its status (match finished -> update refused)
            var match = await MatchRepository.GetById(id) ?? throw new NotFoundException($"Match {id} not found");
            if (match.IsFinished) throw new UnauthorizedException("Cannot update a finished match");
            var targetFieldPlayers = isHostTeam ? match.HostsStartingPlayers : match.VisitorsStartingPlayers;
            if (removedStartingPlayers.Any(player => !targetFieldPlayers.Select(matchEncoder => matchEncoder.Id).Contains(player)))
                throw new BadRequestException("Trying to remove players who are not already on field");

            var teamId = isHostTeam ? match.HostsId : match.VisitorsId;

            ValidateRequestIds(newStartingPlayers, removedStartingPlayers,
                "A player cannot be added nor removed multiple times in the same request",
                "Cannot add and remove the same player in the same request");

            var playersDictionary = await FetchAndValidateCollections(newStartingPlayers, removedStartingPlayers, PlayerRepository.GetById,
                player => player.TeamId.HasValue && player.TeamId.Equals(teamId),
                "Player {0} not found",
                $"Tried adding or removing players who are not part of the {(isHostTeam ? "host" : "visitor")} team");

            // Ensuring that the number of players on field does not exceed the limit for a match, and that a change occurring while a match is being played involves the same number of leaving players and incoming players
            if ((newStartingPlayers.Count() > Match.MaxPlayersPerTeam || removedStartingPlayers.Count() > Match.MaxPlayersPerTeam)
                 && (match.HasStarted ^ newStartingPlayers.Count() != removedStartingPlayers.Count())) throw new BadRequestException($"Players on field for a team cannot exceed the number of {Match.MaxPlayersPerTeam} nor be less than 0");

            var leavingPlayers = removedStartingPlayers.Select(playerId => playersDictionary[playerId]);
            var incomingPlayers = newStartingPlayers.Select(playerId => playersDictionary[playerId]);
            match = isHostTeam
                ? await MatchRepository.RemoveHostStartingPlayers(match, leavingPlayers)
                : await MatchRepository.RemoveVisitorStartingPlayers(match, leavingPlayers);
            match = isHostTeam
                ? await MatchRepository.AddHostStartingPlayers(match, incomingPlayers)
                : await MatchRepository.AddVisitorStartingPlayers(match, incomingPlayers);
            return MatchMapper.ConvertToDto(match);
        }

        private static void ValidateRequestIds(IEnumerable<Guid> newEntities, IEnumerable<Guid> removedEntities, string duplicateError, string conflictError)
        {
            if (newEntities.Distinct().Count() < newEntities.Count() || removedEntities.Distinct().Count() < removedEntities.Count())
                throw new BadRequestException(duplicateError);

            if (newEntities.Intersect(removedEntities).Any())
                throw new BadRequestException(conflictError);
        }
    }
}
