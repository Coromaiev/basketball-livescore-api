using BasketBall_LiveScore.Exceptions;
using BasketBall_LiveScore.Mappers;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Repositories;
using System.Diagnostics;

namespace BasketBall_LiveScore.Services.Impl
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository TeamRepository;
        private readonly ITeamMapper TeamMapper;

        public TeamService(ITeamRepository teamRepository, ITeamMapper teamMapper)
        {
            TeamRepository = teamRepository;
            TeamMapper = teamMapper;
        }

        public async Task<TeamDto> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new BadRequestException("Provided team name is invalid");
            var team = TeamMapper.ConvertToEntity(name);
            await TeamRepository.Create(team);
            return TeamMapper.ConvertToDto(team);
        }

        public async Task Delete(Guid id)
        {
            var team = await TeamRepository.GetById(id) ?? throw new ConflictException($"Team {id} does not exist or has already been deleted");
            await TeamRepository.Delete(team);
        }

        public async IAsyncEnumerable<TeamDto> GetAll()
        {
            var teams = TeamRepository.GetAll() ?? throw new NotFoundException("No teams currently available");
            await foreach (var team in teams) yield return TeamMapper.ConvertToDto(team);
        }

        public async Task<TeamDto> GetById(Guid id)
        {
            var team = await TeamRepository.GetById(id) ?? throw new NotFoundException($"Could not find team with id {id}");
            return TeamMapper.ConvertToDto(team);
        }

        public async Task<TeamDto> Update(Guid id, string newName)
        {
            if (string.IsNullOrEmpty(newName)) throw new BadRequestException("Provided new name is invalid");
            var team = await TeamRepository.GetById(id) ?? throw new NotFoundException($"Could not find team with id {id}");
            team = await TeamRepository.Update(team, newName);
            return TeamMapper.ConvertToDto(team);
        }
    }
}
