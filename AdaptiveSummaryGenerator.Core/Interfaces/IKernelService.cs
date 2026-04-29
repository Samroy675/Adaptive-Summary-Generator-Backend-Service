using AdaptiveSummaryGenerator.Core.Models.Requests;
using AdaptiveSummaryGenerator.Core.Models.Responses;

namespace AdaptiveSummaryGenerator.Core.Interfaces
{
    public interface IKernelService
    {
        Task<SummaryGenerationResponse> GenerateAdaptiveSummaryAsync(SummaryGenerationRequest request);
    }
}
