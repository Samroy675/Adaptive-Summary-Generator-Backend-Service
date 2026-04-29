using AdaptiveSummaryGenerator.Core.Models.Requests;

namespace AdaptiveSummaryGenerator.Core.Interfaces
{
    public interface IRequestValidationService
    {
        void Validate(SummaryGenerationRequest request);
    }
}
