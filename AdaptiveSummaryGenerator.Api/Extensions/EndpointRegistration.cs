using AdaptiveSummaryGenerator.Api.Endpoints;

namespace AdaptiveSummaryGenerator.Api.Extensions;

public static class EndpointRegistration
{
    public static void MapApplicationEndpoints(this WebApplication app)
    {
        app.MapSummaryEndpoints();
    }
}