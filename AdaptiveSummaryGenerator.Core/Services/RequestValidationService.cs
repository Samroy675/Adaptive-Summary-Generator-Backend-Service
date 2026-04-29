using AdaptiveSummaryGenerator.Core.Interfaces;
using AdaptiveSummaryGenerator.Core.Models.Requests;

namespace AdaptiveSummaryGenerator.Core.Services
{
    public class RequestValidationService : IRequestValidationService
    {
        public void Validate(SummaryGenerationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.InputText))
                throw new ArgumentException("Input text is required");
        }
    }
}
