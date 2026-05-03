namespace AdaptiveSummaryGenerator.UI.Models;

// Matches your backend POST /api/summary/generate request body
public class SummaryRequest
{
    public string InputText { get; set; } = string.Empty;
    public string SummaryLength { get; set; } = "Medium";   // Short | Medium | Detailed
    public string SummaryFocus { get; set; } = "Auto";      // Auto | Technical | Business | General | KeyPoints
    public string OutputFormat { get; set; } = "Paragraph"; // Paragraph | BulletPoints
}

// Matches your backend response shape
public class SummaryResponse
{
    public string GeneratedSummary { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string DetectedFocus { get; set; } = string.Empty;
    public string LengthUsed { get; set; } = string.Empty;
    public string FormatUsed { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.Now;
}

// Used by ThoughtProcessPanel component
public class ThoughtStep
{
    public string State { get; set; } = "idle"; // idle | active | done
    public string Html { get; set; } = string.Empty;
}
