using System.Net;

namespace BasketBall_LiveScore.Exceptions
{
    public class ConflictException(string message) : ApiException(message, HttpStatusCode.Conflict)
    {
    }
}
