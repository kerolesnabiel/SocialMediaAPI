using System.Net;

namespace SocialMediaDomain.Exceptions;

public abstract class AppException(
    string message, 
    HttpStatusCode statusCode = HttpStatusCode.InternalServerError) 
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
