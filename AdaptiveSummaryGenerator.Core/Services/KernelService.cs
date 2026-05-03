using AdaptiveSummaryGenerator.Core.Interfaces;
using AdaptiveSummaryGenerator.Core.Models.Requests;
using AdaptiveSummaryGenerator.Core.Models.Responses;
using AdaptiveSummaryGenerator.Core.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace AdaptiveSummaryGenerator.Core.Services
{
    public class KernelService : IKernelService
    {
        private readonly Kernel _kernel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KernelService> _logger;
        private readonly IRequestValidationService _requestValidationService;

        private bool _pluginsAdded = false;

        public KernelService(
            Kernel kernel, 
            IServiceProvider serviceProvider,
            ILogger<KernelService> logger,
            IRequestValidationService requestValidationService)
        {
            _kernel = kernel;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _requestValidationService = requestValidationService;

            EnsurePluginsAdded();

        }

        private void EnsurePluginsAdded()
        {
            if (_pluginsAdded) return;
            var plugin = _serviceProvider.GetRequiredService<SummaryPlugin>();
            _kernel.Plugins.AddFromObject(plugin);
            _pluginsAdded = true;
        }


        public async Task<SummaryGenerationResponse> GenerateAdaptiveSummaryAsync(SummaryGenerationRequest request)
        {
            _logger.LogInformation("========== NEW SUMMARY REQUEST ==========");
            _logger.LogInformation("Input Text: {InputText}", request.InputText);
            _logger.LogInformation("Requested Length: {SummaryLength}", request.SummaryLength);
            _logger.LogInformation("Requested Focus: {SummaryFocus}", request.SummaryFocus);
            _logger.LogInformation("Requested Format: {OutputFormat}", request.OutputFormat);

            try
            {
                var summaryPlugin = _serviceProvider.GetRequiredService<SummaryPlugin>();

                string resolvedFocus = request.SummaryFocus.ToString();
                if(request.SummaryFocus == Enums.SummaryFocusType.Auto)
                {
                    resolvedFocus = await summaryPlugin.AnalyzeContentNatureAsync(request.InputText);

                    _logger.LogInformation("Auto mode selected. AI classified content as: {ResolvedFocus}", resolvedFocus);
                }

                _requestValidationService.Validate(request);
                EnsurePluginsAdded();
                var prompt = BuildSummaryPrompt(request, resolvedFocus);

                _logger.LogInformation("Resolved Summary Focus Used For Prompt Routing: {ResolvedFocus}", resolvedFocus);
                _logger.LogInformation("Prompt sent to Semantic Kernel orchestrator.");

                var settings = new OpenAIPromptExecutionSettings
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                    Temperature = 0.1,
                    TopP = 0.8
                };

                var arguments = new KernelArguments
                {
                    ExecutionSettings = new Dictionary<string, PromptExecutionSettings>
                    {
                        ["default"] = settings
                    }
                };

                var result = await _kernel.InvokePromptAsync(prompt, arguments);

                var responseText = result.GetValue<string>() ?? string.Empty;

                _logger.LogInformation("Generated Final Summary: {GeneratedSummary}", responseText);
                _logger.LogInformation("========== REQUEST COMPLETED ==========");

                return new SummaryGenerationResponse
                {
                    GeneratedSummary = responseText,
                    IsSuccess = true,
                    Message = "Summary generated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating adaptive summary.");

                return new SummaryGenerationResponse
                {
                    GeneratedSummary = string.Empty,
                    IsSuccess = false,
                    Message = $"Error generating summary: {ex.ToString()}"
                };
            }
        }

        private string BuildSummaryPrompt(SummaryGenerationRequest request, string resolvedFocus)
        {
            return $@"
            You are an orchestration agent.

            Available functions:
            - GenerateGeneralSummary(inputText, summaryLength, outputFormat)
            - GenerateTechnicalSummary(inputText, summaryLength, outputFormat)
            - GenerateBusinessSummary(inputText, summaryLength, outputFormat)
            - GenerateKeyPointSummary(inputText, summaryLength, outputFormat)

            Rules:
            - Select exactly one function based on SummaryFocus.
            - Never generate summary yourself.
            - Always invoke one function.

           Function Inputs:
           inputText = {request.InputText}
           summaryLength = {request.SummaryLength}
           outputFormat = {request.OutputFormat}

           Requested SummaryFocus: {resolvedFocus}";
        }
    }
}
