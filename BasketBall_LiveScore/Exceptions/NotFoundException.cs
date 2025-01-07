using System.Net;

namespace BasketBall_LiveScore.Exceptions
{
    public class NotFoundException(string message) : ApiException(message, HttpStatusCode.NotFound)
    {
    }
}
