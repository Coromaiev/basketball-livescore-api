using System.Net;

namespace BasketBall_LiveScore.Exceptions
{
    public class UnauthorizedException(string message) : ApiException(message, HttpStatusCode.Unauthorized)
    {
    }
}
