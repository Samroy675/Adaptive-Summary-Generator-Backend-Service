using AdaptiveSummaryGenerator.Core.Interfaces;
using System.Text;

namespace AdaptiveSummaryGenerator.Core.Services
{
    public class PromptLoaderService : IPromptLoaderService
    {
        public async Task<string> LoadAsync(string promptName)
        {
           if(string.IsNullOrWhiteSpace(promptName))
            {
                throw new ArgumentException("Prompt name cannot be null.", nameof(promptName));
            }

           var assembly = typeof(PromptLoaderService).Assembly;

           var resourceName = $"AdaptiveSummaryGenerator.Core.Prompts.{promptName}.prompt.txt";

            await using var stream = assembly.GetManifestResourceStream(resourceName);

            if(stream == null)
            {
                throw new FileNotFoundException($"Prompt'{promptName}' not found as an embedded resource.");
            }

            using var reader = new StreamReader(stream,Encoding.UTF8, 
                detectEncodingFromByteOrderMarks: true);

            return await reader.ReadToEndAsync();
        }
    }
}
