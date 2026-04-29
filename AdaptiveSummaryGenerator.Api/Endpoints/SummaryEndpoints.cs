using AdaptiveSummaryGenerator.Core.Interfaces;
using AdaptiveSummaryGenerator.Core.Models.Requests;

namespace AdaptiveSummaryGenerator.Api.Endpoints;

public static class SummaryEndpoints
{
    public static void MapSummaryEndpoints(this WebApplication app)
    {
        app.MapPost("/api/summary/generate", async Task<IResult> (
            SummaryGenerationRequest request,
            IKernelService kernelService) =>
        {
            if (request == null)
                return Results.BadRequest("Invalid request.");

            var result = await kernelService.GenerateAdaptiveSummaryAsync(request);

            return Results.Ok(result);
        })
        .WithName("GenerateAdaptiveSummary")
        .WithOpenApi();

        app.MapGet("/api/debug/ip", async () =>
        {
            using var httpClient = new HttpClient();
            var ip = await httpClient.GetStringAsync("http://api.ipify.org");
            return Results.Ok(new { OutboundIp = ip });
        });
    }
}