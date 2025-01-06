using BasketBall_LiveScore.Exceptions;
using BasketBall_LiveScore.Models;
using BasketBall_LiveScore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BasketBall_LiveScore.Controllers
{
    [Route("api/matchEvents")]
    [ApiController]
    public class MatchEventController : ControllerBase
    {
        private readonly IMatchEventService MatchEventService;
        private readonly ILogger<MatchEventController> Logger;

        public MatchEventController(IMatchEventService matchEventService, ILogger<MatchEventController> logger)
        {
            MatchEventService = matchEventService;
            Logger = logger;
        }

        [Route("{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            try
            {
                return Ok(await MatchEventService.GetById(id));
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

        [Route("match/{id:guid}")]
        [HttpGet]
        public async Task<IActionResult> GetFromMatch([FromRoute] Guid id)
        {
            try
            {
                return Ok(await MatchEventService.GetAllEventsByMatch(id));
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

        [Route("match/{id:guid}:{type:alpha}")]
        [HttpGet]
        public async Task<IActionResult> GetFromMatchWithType([FromRoute] Guid id, [FromRoute] string type)
        {
            try
            {
                var eventsList = new List<MatchEventDto>();
                IAsyncEnumerable<MatchEventDto>? events = type switch
                {
                    "fault" => MatchEventService.GetEventsByMatch<Fault, FaultDto>(id),
                    "playerChange" => MatchEventService.GetEventsByMatch<PlayerChange, PlayerChangeDto>(id),
                    "scoreChange" => MatchEventService.GetEventsByMatch<ScoreChange, ScoreChangeDto>(id),
                    "timeout" => MatchEventService.GetEventsByMatch<TimeOut, TimeOutDto>(id),
                    _ => null
                };
                if (events is null) return BadRequest($"Unknown event type : {type}");
                await foreach (var matchEvent in events)
                {
                    eventsList.Add(matchEvent);
                }
                return Ok(eventsList);
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

        [Route("faults")]
        [HttpPost]
        [Authorize(Policy = "EncoderAccess")]
        public async Task<IActionResult> CreateFault([FromBody] FaultCreateDto faultDto)
        {
            try
            {
                var newFault = await MatchEventService.CreateFault(faultDto);
                return CreatedAtAction(nameof(Get), new { id = newFault.Id }, newFault);
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

        [Route("playerChanges")]
        [HttpPost]
        [Authorize(Policy = "EncoderAccess")]
        public async Task<IActionResult> CreatePlayerChange([FromBody] PlayerChangeCreateDto playerChangeDto)
        {
            try
            {
                var newPlayerChange = await MatchEventService.CreatePlayerChange(playerChangeDto);
                return CreatedAtAction(nameof(Get), new { id = newPlayerChange.Id }, newPlayerChange);
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

        [Route("scoreChanges")]
        [HttpPost]
        [Authorize(Policy = "EncoderAccess")]
        public async Task<IActionResult> CreateScoreChange([FromBody] ScoreChangeCreateDto scoreChangeDto)
        {
            try
            {
                var newScoreChange = await MatchEventService.CreateScoreChange(scoreChangeDto);
                return CreatedAtAction(nameof(Get), new { id = newScoreChange.Id }, newScoreChange);
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

        [Route("timeouts")]
        [HttpPost]
        [Authorize(Policy = "EncoderAccess")]
        public async Task<IActionResult> CreateTimeOut([FromBody] TimeOutCreateDto timeOutDto)
        {
            try
            {
                var newTimeOut = await MatchEventService.CreateTimeOut(timeOutDto);
                return CreatedAtAction(nameof(Get), new { id = newTimeOut.Id }, newTimeOut);
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
        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] MatchEventUpdateDto updateDto)
        {
            try
            {
                await MatchEventService.UpdateEvent(id, updateDto);
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
                await MatchEventService.DeleteEvent(id);
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
    }
}

