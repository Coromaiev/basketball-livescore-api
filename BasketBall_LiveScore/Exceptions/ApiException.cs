using System.Net;

namespace BasketBall_LiveScore.Exceptions
{
    public class ApiException(string message, HttpStatusCode statusCode) : Exception(message)
    {
        public readonly HttpStatusCode StatusCode = statusCode;
    }
}
