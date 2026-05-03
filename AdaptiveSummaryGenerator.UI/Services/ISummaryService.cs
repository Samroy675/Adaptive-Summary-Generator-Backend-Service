using AdaptiveSummaryGenerator.UI.Models;

namespace AdaptiveSummaryGenerator.UI.Services;

// ─── Interface ───────────────────────────────────────────────────────────────
// When you're ready to integrate: create RealSummaryService : ISummaryService
// and call your backend HTTP endpoint instead of dummy data.
// Then swap registration in Program.cs: builder.Services.AddScoped<ISummaryService, RealSummaryService>();
public interface ISummaryService
{
    // regenerateCount removed — Regenerate simply re-calls with same request.
    // SK Temperature/TopP handles variation naturally on the backend.
    Task<SummaryResponse> GenerateSummaryAsync(SummaryRequest request);
}
