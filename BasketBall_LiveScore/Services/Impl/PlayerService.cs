using BasketBall_LiveScore.Exceptions;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Repositories;
using System.Data.SqlTypes;

namespace BasketBall_LiveScore.Services.Impl
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository PlayerRepository;
        private readonly ITeamRepository TeamRepository;

        public PlayerService(IPlayerRepository playerRepository, ITeamRepository teamRepository)
        {
            PlayerRepository = playerRepository;
            TeamRepository = teamRepository;
        }

        public async Task<PlayerDto?> Create(PlayerCreateDto player)
        {
            Team? team = null;
            if (player.TeamId.HasValue && player.Number.HasValue)
            {
                team = await TeamRepository.GetById(player.TeamId.Value) ?? throw new NotFoundException("Could not find team of new player");
                if (!IsNumberValidForTeam(team, player.Number.Value))
                {
                    throw new ConflictException($"Provided player number is already used among team {team.Name}");
                }
            }
            else if (player.TeamId.HasValue) throw new BadRequestException("Number is required when assigning a player to a team");
            
            var newPlayer = ConvertToEntity(player);
            newPlayer.Team = team;
            newPlayer = await PlayerRepository.Create(newPlayer);
            if (team is not null) await TeamRepository.AddPlayer(team, newPlayer);
            return ConvertToDto(newPlayer);
        }

        public async Task Delete(Guid id)
        {
            var player = await PlayerRepository.GetById(id) ?? throw new ConflictException($"Player {id} does not exist or has already been deleted");
            if (player.Team is not null)
            {
                await TeamRepository.RemovePlayer(player.Team, player);
            }
            await PlayerRepository.Delete(player);
        }

        public async IAsyncEnumerable<PlayerDto?> GetAll()
        {
            var players = PlayerRepository.GetAll() ?? throw new NotFoundException("No players currently available");
            await foreach (var player in players)
            {
                yield return ConvertToDto(player);
            }
        }

        public async Task<PlayerDto?> GetById(Guid id)
        {
            var player = await PlayerRepository.GetById(id) ?? throw new NotFoundException($"Player with id {id} not found");
            return ConvertToDto(player);
        }

        public async IAsyncEnumerable<PlayerDto?> GetByTeam(Guid teamId)
        {
            var team = await TeamRepository.GetById(teamId) ?? throw new NotFoundException($"Team {teamId} does not exist");
            var teamPlayers = PlayerRepository.GetByTeam(teamId) ?? throw new NotFoundException($"Team {teamId} does not have any player");
            await foreach (var player in teamPlayers)
            {
                yield return ConvertToDto(player);
            }
        }

        public async Task<PlayerDto?> Update(Guid id, PlayerUpdateDto updatedPlayer)
        {
            var playerToUpdate = await PlayerRepository.GetById(id) ?? throw new NotFoundException($"Player {id} not found");

            // Trying to perform a player team transfer
            if (updatedPlayer.TeamId.HasValue && updatedPlayer.Number.HasValue)
            {
                var newTeam = await TeamRepository.GetById(updatedPlayer.TeamId.Value) ?? throw new NotFoundException($"Could not find team {updatedPlayer.TeamId.Value}");
                if (!IsNumberValidForTeam(newTeam, updatedPlayer.Number.Value))
                    throw new ConflictException($"The number {updatedPlayer.Number.Value} is already assigned in team {newTeam.Id}");
                if (playerToUpdate.Team is not null)
                {
                    await TeamRepository.RemovePlayer(playerToUpdate.Team, playerToUpdate);
                }
                await TeamRepository.AddPlayer(newTeam, playerToUpdate);
            } // Else : trying to update the player number inside their team
            else if (updatedPlayer.Number.HasValue && playerToUpdate.Team is not null && !IsNumberValidForTeam(playerToUpdate.Team, updatedPlayer.Number.Value))
            {
                throw new BadRequestException($"Number {updatedPlayer.Number.Value} is already assigned in team {playerToUpdate.Team.Id}");
            } // Else : trying to perform a player team transfer without providing a number into that new team
            else if ((updatedPlayer.TeamId.HasValue && !updatedPlayer.Number.HasValue) || (updatedPlayer.Number.HasValue && playerToUpdate.Team is null))
                throw new BadRequestException("A number value is required when assigning a player to a new team. A number cannot be assigned to a player without a team");

            var player = await PlayerRepository.Update
                (
                    playerToUpdate,
                    updatedPlayer.Number,
                    updatedPlayer.TeamId.HasValue ? await TeamRepository.GetById(updatedPlayer.TeamId.Value) : null,
                    updatedPlayer.FirstName,
                    updatedPlayer.LastName
                );
            return ConvertToDto(player);
        }

        public async Task<PlayerDto?> UpdateQuitTeam(Guid id)
        {
            var playerToUpdate = await PlayerRepository.GetById(id) ?? throw new NotFoundException($"Player with id {id} not found");
            if (playerToUpdate.Team is null) throw new ConflictException($"Cannot remove player {id} from their team. No team assigned");
            await TeamRepository.RemovePlayer(playerToUpdate.Team, playerToUpdate);
            playerToUpdate = await PlayerRepository.RemoveTeam(playerToUpdate);
            return ConvertToDto(playerToUpdate);
        }

        private static PlayerDto ConvertToDto(Player player)
        {
            PlayerDto playerDto = new(
                    player.Id,
                    player.FirstName,
                    player.LastName,
                    player.TeamId,
                    player.Number
                );
            return playerDto;
        }

        private static Player ConvertToEntity(PlayerCreateDto playerDto)
        {
            Player player = new()
            {
                FirstName = playerDto.FirstName,
                LastName = playerDto.LastName,
                TeamId = playerDto.TeamId,
                Number = playerDto.Number,
            };
            return player;
        }

        private static bool IsNumberValidForTeam(Team team, byte number) => !team.Players.Any(player => player.Number == number);
    }
}
