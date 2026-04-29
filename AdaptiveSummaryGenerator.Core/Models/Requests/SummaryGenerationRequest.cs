using AdaptiveSummaryGenerator.Core.Enums;

namespace AdaptiveSummaryGenerator.Core.Models.Requests
{
    public class SummaryGenerationRequest
    {
        public string InputText { get; set; } = string.Empty; 
        public SummaryLengthType SummaryLength { get; set; } = SummaryLengthType.Medium;
        public SummaryFocusType SummaryFocus { get; set; } = SummaryFocusType.General;
        public OutputFormatType OutputFormat { get; set; } = OutputFormatType.Paragraph;
    }
}
