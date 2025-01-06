using BasketBall_LiveScore.Exceptions;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BasketBall_LiveScore.Controllers
{
    [Route("api/matchs")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService MatchService;
        private readonly ILogger<MatchController> Logger;

        public MatchController(IMatchService matchService, ILogger<MatchController> logger)
        {
            MatchService = matchService;
            Logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await ProcessDataCollection<object>(null, null));
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all matchs");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("encoder/{id:guid}")]
        [HttpGet]
        [Authorize(Policy = "EncoderAccess")]
        public async Task<IActionResult> GetWithEncoder([FromRoute] Guid id)
        {
            try
            {
                return Ok(await ProcessDataCollection(MatchService.GetByEncoder, id));
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all matchs");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("team/{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> GetWithTeam([FromRoute] Guid id)
        {
            try
            {
                return Ok(await ProcessDataCollection<Guid>(MatchService.GetByTeam, id));
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all matchs");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("finished/{isFinished:bool}")]
        [HttpGet]
        public async Task<IActionResult> GetWithEndStatus([FromRoute] bool isFinished)
        {
            try
            {
                return Ok(await ProcessDataCollection<bool>(MatchService.GetWithEndStatus, isFinished));
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all matchs");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            try
            {
                return Ok(await MatchService.GetById(id));
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all matchs");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Authorize(Policy = "EncoderAccess")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MatchCreateDto match)
        {
            try
            {
                Console.WriteLine($"{match.HostsId} - {match.VisitorsId}");
                var newMatch = await MatchService.Create(match);
                return CreatedAtAction(nameof(Get), new { id = newMatch.Id }, newMatch);
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all matchs");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Authorize(Policy = "MatchAssignmentPolicy")]
        [Route("{id}/prep")]
        [HttpPut]
        public async Task<IActionResult> UpdatePrep([FromRoute] Guid id, [FromBody] MatchUpdatePrepDto matchUpdateDto)
        {
            try
            {
                await MatchService.UpdatePrepDetails(id, matchUpdateDto);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all matchs");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("{id:guid}/play")]
        [HttpPut]
        [Authorize(Policy = "MatchAssignmentPolicy")]
        public async Task<IActionResult> UpdatePlay([FromRoute] Guid id, [FromBody] MatchUpdatePlayDto matchUpdatePlayDto)
        {
            try
            {
                await MatchService.UpdatePlayDetails(id, matchUpdatePlayDto);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all matchs");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("{id:guid}/encoders")]
        [HttpPut]
        [Authorize(Policy = "EncoderAccess")]
        public async Task<IActionResult> UpdateEncoders([FromRoute] Guid id, [FromBody] MatchUpdateListDto playEncodersChanges)
        {
            try
            {
                await MatchService.UpdatePlayEncoders(id, playEncodersChanges);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all matchs");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("{id:guid}/players:{team:alpha}")]
        [HttpPut]
        [Authorize(Policy = "MatchAssignmentPolicy")]
        public async Task<IActionResult> UpdateTeamPlayers([FromRoute] Guid id, [FromRoute] string team, MatchUpdateListDto playersChanges)
        {
            try
            {
                switch (team)
                {
                    case "hosts":
                        await MatchService.UpdateHostsStartingPlayers(id, playersChanges);
                        break;
                    case "visitors":
                        await MatchService.UpdateVisitorsStartingPlayers(id, playersChanges);
                        break;
                    default:
                        return BadRequest($"Unknown players' team argument : {team}");
                }
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all matchs");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        [Route("{id:guid}")]
        [HttpDelete]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                await MatchService.Delete(id);
                return NoContent();
            }
            catch (ApiException ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Exception while fetching all matchs");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
            }
        }

        private async Task<List<MatchDto>> ProcessDataCollection<T>(Func<T, IAsyncEnumerable<MatchDto>>? fetchFunc, T? value)
        {
            var matchs = value is not null && fetchFunc is not null ? fetchFunc(value) : MatchService.GetAll();
            var matchList = new List<MatchDto>();
            await foreach(var match in matchs)
            {
                matchList.Add(match);
            }
            return matchList;
        }
    }
}
