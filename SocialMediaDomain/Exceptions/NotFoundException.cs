using System.Net;

namespace SocialMediaDomain.Exceptions;

public class NotFoundException(string resourceType, string resourceIdentifier)
    : AppException($"{resourceType} with Id: {resourceIdentifier} doesn't exist", HttpStatusCode.NotFound)
{
}
