using BasketBall_LiveScore.Exceptions;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Repositories.Impl;
using BasketBall_LiveScore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BasketBall_LiveScore.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService PlayerService;
        private readonly ILogger<PlayerController> Logger;

        public PlayerController(IPlayerService playerService, ILogger<PlayerController> logger)
        {
            PlayerService = playerService;
            Logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] Guid? teamId)
        {
            try
            {
                var playersList = new List<PlayerDto>();
                var players = teamId.HasValue ? PlayerService.GetByTeam(teamId.Value) : PlayerService.GetAll();
                await foreach (var player in players)
                {
                    playersList.Add(player);
                }
                return Ok(players);
            } 
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching players");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlayerCreateDto playerDto)
        {
            try
            {
                var player = await PlayerService.Create(playerDto);
                return CreatedAtAction(nameof(Get), new { id = player.Id }, player);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex?.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception during player creation");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            try
            {
                var player = await PlayerService.GetById(id);
                return Ok(player);
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
        public async Task<IActionResult> Update([FromRoute] Guid id, PlayerUpdateDto playerDto)
        {
            try
            {
                var updatedPlayer = await PlayerService.Update(id, playerDto);
                return NoContent();
            } 
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while performing player update");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("{id:guid}/leave")]
        [HttpPut]
        public async Task<IActionResult> LeavePlayerTeam([FromRoute] Guid id)
        {
            try
            {
                var updatedPlayer = await PlayerService.UpdateQuitTeam(id);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while performing player update");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("{id:guid}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                await PlayerService.Delete(id);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, $"An error occurred trying to delete player {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occured");
            }
        }
    }
}
