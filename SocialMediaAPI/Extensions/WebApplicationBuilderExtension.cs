using Microsoft.OpenApi;
using Serilog;
using SocialMediaAPI.Handlers;

namespace SocialMediaAPI.Extensions;

public static class WebApplicationBuilderExtension
{
    public static void AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication();
        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearerAuth", document)] = []
            });

            // To avoid conflict between identity route and controller route
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.Last());
        });

        builder.Services.AddProblemDetails(options => options.CustomizeProblemDetails = ctx =>
                {
                    ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
                    ctx.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
                    ctx.ProblemDetails.Instance = $"{ctx.HttpContext.Request.Method} {ctx.HttpContext.Request.Path}";
                });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));
    }
}
