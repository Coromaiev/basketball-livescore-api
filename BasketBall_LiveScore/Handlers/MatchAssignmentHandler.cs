using BasketBall_LiveScore.Requirements;
using BasketBall_LiveScore.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BasketBall_LiveScore.Handlers
{
    public class MatchAssignmentHandler : AuthorizationHandler<MatchAssignmentRequirement>
    {
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly IMatchService MatchService;

        public MatchAssignmentHandler(IHttpContextAccessor httpContextAccessor, IMatchService matchService)
        {
            HttpContextAccessor = httpContextAccessor;
            MatchService = matchService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MatchAssignmentRequirement requirement)
        {
            if (!Guid.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                context.Fail();
                return Task.CompletedTask;
            }
            var userRole = context.User.FindFirstValue(ClaimTypes.Role);

            if (!userRole.Equals("Encoder"))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var routeData = HttpContextAccessor.HttpContext?.Request.RouteValues;
            if (
                routeData is null ||
                !routeData.TryGetValue("id", out var routeMatchId) || 
                !Guid.TryParse(routeMatchId?.ToString(), out var matchId) || 
                !routeData.TryGetValue("action", out var action)
               )
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var match = MatchService.GetById(matchId).Result;
            bool isPrepEncoder = match.PrepEncoderId.Equals(userId);
            bool isPlayEncoder = match.PlayEncoders.Any(encoderId => encoderId.Equals(userId));

            if ((action.Equals("prepare") && isPrepEncoder) || (action.Equals("play") && isPlayEncoder))
            {
                context.Succeed(requirement);
            } else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
