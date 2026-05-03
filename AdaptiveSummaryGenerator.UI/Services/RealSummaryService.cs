using AdaptiveSummaryGenerator.UI.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdaptiveSummaryGenerator.UI.Services;

// ─── REAL SERVICE ─────────────────────────────────────────────────────────────
// Calls the actual backend: POST /api/summary/generate
// Backend expects enums as strings (JsonStringEnumConverter is configured there).
// Frontend sends strings too — no enum mismatch.
public class RealSummaryService : ISummaryService
{
    private readonly HttpClient _http;
    private readonly ILogger<RealSummaryService> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public RealSummaryService(HttpClient http, ILogger<RealSummaryService> logger)
    {
        _http   = http;
        _logger = logger;
    }

    public async Task<SummaryResponse> GenerateSummaryAsync(SummaryRequest request)
    {
        // Regenerate just re-calls this method with same request.
        // SK's Temperature=0.1 + TopP=0.8 produces natural variation each call.
        try
        {
            _logger.LogInformation("Calling backend: POST /api/summary/generate");

            // Backend expects SummaryFocus, SummaryLength, OutputFormat as strings
            // matching enum names exactly (Auto, Technical, Business, etc.)
            var payload = new
            {
                inputText    = request.InputText,
                summaryLength = request.SummaryLength,   // "Short" | "Medium" | "Detailed"
                summaryFocus  = request.SummaryFocus,    // "Auto" | "Technical" | "Business" | "General" | "KeyPoints"
                outputFormat  = request.OutputFormat     // "Paragraph" | "BulletPoints"
            };

            var httpResponse = await _http.PostAsJsonAsync("api/summary/generate", payload);

            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogError("Backend returned {Status}: {Error}", httpResponse.StatusCode, error);

                return new SummaryResponse
                {
                    IsSuccess        = false,
                    GeneratedSummary = string.Empty,
                    Message          = $"Backend error ({httpResponse.StatusCode}): {error}"
                };
            }

            // Backend returns: { generatedSummary, isSuccess, message }
            var backendResponse = await httpResponse.Content.ReadFromJsonAsync<BackendResponse>(_jsonOptions);

            if (backendResponse is null)
            {
                return new SummaryResponse
                {
                    IsSuccess = false,
                    Message   = "Empty response from backend."
                };
            }

            // Map backend response → frontend SummaryResponse
            // DetectedFocus / LengthUsed / FormatUsed are echoed from the request
            // since the backend doesn't return them separately.
            return new SummaryResponse
            {
                GeneratedSummary = backendResponse.GeneratedSummary,
                IsSuccess        = backendResponse.IsSuccess,
                Message          = backendResponse.Message,
                DetectedFocus    = request.SummaryFocus == "Auto" ? "Auto-detected" : request.SummaryFocus,
                LengthUsed       = request.SummaryLength,
                FormatUsed       = request.OutputFormat,
                GeneratedAt      = DateTime.Now
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error calling backend.");
            return new SummaryResponse
            {
                IsSuccess        = false,
                GeneratedSummary = string.Empty,
                Message          = "Could not reach backend. Make sure the API is running on http://localhost:5183."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error.");
            return new SummaryResponse
            {
                IsSuccess        = false,
                GeneratedSummary = string.Empty,
                Message          = $"Unexpected error: {ex.Message}"
            };
        }
    }

    // Maps the backend's SummaryGenerationResponse shape
    private class BackendResponse
    {
        public string GeneratedSummary { get; set; } = string.Empty;
        public bool   IsSuccess        { get; set; }
        public string Message          { get; set; } = string.Empty;
    }
}
