using BasketBall_LiveScore.Exceptions;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Repositories;
using System.Diagnostics;

namespace BasketBall_LiveScore.Services.Impl
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository TeamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            TeamRepository = teamRepository;
        }

        public async Task<TeamDto?> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new BadRequestException("Provided team name is invalid");
            var team = ConvertToEntity(name);
            await TeamRepository.Create(team);
            return ConvertToDto(team);
        }

        public async Task Delete(Guid id)
        {
            var team = await TeamRepository.GetById(id) ?? throw new ConflictException($"Team {id} does not exist or has already been deleted");
            await TeamRepository.Delete(team);
        }

        public async IAsyncEnumerable<TeamDto?> GetAll()
        {
            var teams = TeamRepository.GetAll() ?? throw new NotFoundException("No teams currently available");
            await foreach (var team in teams) yield return ConvertToDto(team);
        }

        public async Task<TeamDto?> GetById(Guid id)
        {
            var team = await TeamRepository.GetById(id) ?? throw new NotFoundException($"Could not find team with id {id}");
            return ConvertToDto(team);
        }

        public async Task<TeamDto?> Update(Guid id, string newName)
        {
            if (string.IsNullOrEmpty(newName)) throw new BadRequestException("Provided new name is invalid");
            var team = await TeamRepository.GetById(id) ?? throw new NotFoundException($"Could not find team with id {id}");
            team = await TeamRepository.Update(team, newName);
            return ConvertToDto(team);
        }

        private static Team ConvertToEntity(string name)
        {
            var team = new Team
            {
                Name = name,
            };
            return team;
        }

        private static TeamDto ConvertToDto(Team team)
        {
            var teamDto = new TeamDto
                (
                    team.Id,
                    team.Name,
                    team.Players.Select(player => new PlayerDto
                    (
                        player.Id,
                        player.FirstName,
                        player.LastName,
                        team.Id,
                        player.Number
                    )).ToList()
                );
            return teamDto;
        }
    }
}
