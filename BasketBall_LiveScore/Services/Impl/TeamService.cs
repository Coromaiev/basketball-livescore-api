using BasketBall_LiveScore.Exceptions;
using BasketBall_LiveScore.Hubs;
using BasketBall_LiveScore.Mappers;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Repositories;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace BasketBall_LiveScore.Services.Impl
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository TeamRepository;
        private readonly IPlayerRepository PlayerRepository;
        private readonly ITeamMapper TeamMapper;
        private readonly IHubContext<TeamHub> HubContext;

        public TeamService(ITeamRepository teamRepository, IPlayerRepository playerRepository, ITeamMapper teamMapper, IHubContext<TeamHub> hubContext)
        {
            TeamRepository = teamRepository;
            PlayerRepository = playerRepository;
            TeamMapper = teamMapper;
            HubContext = hubContext;
        }

        public async Task<TeamDto> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new BadRequestException("Provided team name is invalid");
            var team = TeamMapper.ConvertToEntity(name);
            team = await TeamRepository.Create(team);
            var teamDto = TeamMapper.ConvertToDto(team);
            await HubContext.Clients.All.SendAsync("TeamsUpdated", teamDto);
            return teamDto;
        }

        public async Task Delete(Guid id)
        {
            var team = await TeamRepository.GetById(id) ?? throw new ConflictException($"Team {id} does not exist or has already been deleted");
            foreach (var player in team.Players)
            {
                await PlayerRepository.RemoveTeam(player);
            }
            await TeamRepository.Delete(team);
            await HubContext.Clients.All.SendAsync("TeamRemoved", id);
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
            var teamDto = TeamMapper.ConvertToDto(team);
            await HubContext.Clients.All.SendAsync("TeamsUpdated", teamDto);
            return teamDto;
        }
    }
}
