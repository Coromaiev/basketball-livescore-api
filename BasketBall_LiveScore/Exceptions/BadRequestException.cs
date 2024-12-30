using System.Net;

namespace BasketBall_LiveScore.Exceptions
{
    public class BadRequestException(string message) : ApiException(message, HttpStatusCode.BadRequest)
    {
    }
}
