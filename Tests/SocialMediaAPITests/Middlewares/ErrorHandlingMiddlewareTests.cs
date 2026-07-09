using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SocialMediaAPI.Middlewares;
using SocialMediaDomain.Exceptions;
using Xunit;
using Assert = Xunit.Assert;

namespace SocialMediaAPITests.Middlewares;

public class ErrorHandlingMiddlewareTests
{
    private Mock<ILogger<ErrorHandlingMiddleware>> _logger;
    private ErrorHandlingMiddleware _middleware;
    private DefaultHttpContext _httpContext;

    public ErrorHandlingMiddlewareTests()
    {
        _logger = new Mock<ILogger<ErrorHandlingMiddleware>>();
        _middleware = new ErrorHandlingMiddleware(_logger.Object);
        _httpContext = new DefaultHttpContext();
    }

    [Fact()]
    public async Task InvokeAsync_WhenNoExceptionThrown_CallTheNextDelegate()
    {
        var nextDelegate = new Mock<RequestDelegate>();

        await _middleware.InvokeAsync(_httpContext, nextDelegate.Object);

        nextDelegate.Verify(next => next(_httpContext), Times.Once);
    }

    [Fact()]
    public async Task InvokeAsync_WhenNotFoundExceptionThrown_SetStatusCodeTo404()
    {
        await _middleware.InvokeAsync(_httpContext, _ => throw new NotFoundException("",""));

        Assert.Equal(404, _httpContext.Response.StatusCode);
    }

    [Fact()]
    public async Task InvokeAsync_WhenForbidExceptionThrown_SetStatusCodeTo403()
    {
        await _middleware.InvokeAsync(_httpContext, _ => throw new ForbidException());

        Assert.Equal(403, _httpContext.Response.StatusCode);
    }

    [Fact()]
    public async Task InvokeAsync_WhenIncorrectExceptionThrown_SetStatusCodeTo403()
    {
        await _middleware.InvokeAsync(_httpContext, _ => throw new BadRequestException("Password"));

        Assert.Equal(400, _httpContext.Response.StatusCode);
    }

    [Fact()]
    public async Task InvokeAsync_WhenGeneralExceptionThrown_SetStatusCodeTo500()
    {
        await _middleware.InvokeAsync(_httpContext, _ => throw new Exception());

        Assert.Equal(500, _httpContext.Response.StatusCode);
    }
}