using BasketBall_LiveScore.Exceptions;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BasketBall_LiveScore.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService TeamService;
        private readonly ILogger<TeamController> Logger;

        public TeamController(ITeamService teamService, ILogger<TeamController> logger)
        {
            TeamService = teamService;
            Logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var teamsList = new List<TeamDto>();
                var teams = TeamService.GetAll();
                await foreach (var team in teams)
                {
                    teamsList.Add(team);
                }
                return Ok(teamsList);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all teams");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            try
            {
                var team = await TeamService.GetById(id);
                return Ok(team);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all teams");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string teamName)
        {
            try
            {
                var newTeam = await TeamService.Create(teamName);
                return CreatedAtAction(nameof(Get), new { id = newTeam.Id }, newTeam);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all teams");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("{id:guid}")]
        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] string newName)
        {
            try
            {
                var updatedTeam = await TeamService.Update(id, newName);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all teams");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("{id:guid}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                await TeamService.Delete(id);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all teams");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }
    }
}
