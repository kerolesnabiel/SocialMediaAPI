using System.Net;

namespace SocialMediaDomain.Exceptions;

public class BadRequestException(string message) 
    : AppException(message, HttpStatusCode.BadRequest)
{
}
