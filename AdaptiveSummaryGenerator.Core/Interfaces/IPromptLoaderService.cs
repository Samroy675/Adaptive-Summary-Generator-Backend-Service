namespace AdaptiveSummaryGenerator.Core.Interfaces
{
    public interface IPromptLoaderService
    {
        Task<string> LoadAsync(string promptName);
    }
}
