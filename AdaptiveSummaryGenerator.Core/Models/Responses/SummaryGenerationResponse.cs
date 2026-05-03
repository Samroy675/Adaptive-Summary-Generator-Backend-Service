namespace AdaptiveSummaryGenerator.Core.Models.Responses
{
    public class SummaryGenerationResponse
    {
        public string GeneratedSummary { get; set; } = string.Empty;
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}
