using AdaptiveSummaryGenerator.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;

namespace AdaptiveSummaryGenerator.Core.Plugins;

public class SummaryPlugin
{
    private readonly IPromptLoaderService _promptLoader;
    private readonly Kernel _kernel;
    private readonly ILogger<SummaryPlugin> _logger;

    public SummaryPlugin(IPromptLoaderService promptLoader, Kernel kernel, ILogger<SummaryPlugin> logger)
    {
        _promptLoader = promptLoader;
        _kernel = kernel;
        _logger = logger;
    }

    [KernelFunction("GenerateGeneralSummary")]
    [Description("Generates a general overall summary.")]
    public async Task<string> GenerateGeneralSummaryAsync(string inputText, string summaryLength, string outputFormat)
    {
        var template = await _promptLoader.LoadAsync("general-summary");

        var args = new KernelArguments
        {
            ["inputText"] = inputText,
            ["summaryLength"] = summaryLength,
            ["outputFormat"] = outputFormat
        };

        return await ExecutePromptAsync(template, args, "General");
    }

    [KernelFunction("GenerateTechnicalSummary")]
    [Description("Generates a technical focused summary.")]
    public async Task<string> GenerateTechnicalSummaryAsync(string inputText, string summaryLength, string outputFormat)
    {
        var template = await _promptLoader.LoadAsync("technical-summary");

        var args = new KernelArguments
        {
            ["inputText"] = inputText,
            ["summaryLength"] = summaryLength,
            ["outputFormat"] = outputFormat
        };

        return await ExecutePromptAsync(template, args, "Technical");
    }

    [KernelFunction("GenerateBusinessSummary")]
    [Description("Generates a business focused summary.")]
    public async Task<string> GenerateBusinessSummaryAsync(string inputText, string summaryLength, string outputFormat)
    {
        var template = await _promptLoader.LoadAsync("business-summary");

        var args = new KernelArguments
        {
            ["inputText"] = inputText,
            ["summaryLength"] = summaryLength,
            ["outputFormat"] = outputFormat
        };

        return await ExecutePromptAsync(template, args, "Business");
    }

    [KernelFunction("GenerateKeyPointSummary")]
    [Description("Generates only key point highlights.")]
    public async Task<string> GenerateKeyPointSummaryAsync(string inputText, string summaryLength, string outputFormat)
    {
        var template = await _promptLoader.LoadAsync("keypoint-summary");

        var args = new KernelArguments
        {
            ["inputText"] = inputText,
            ["summaryLength"] = summaryLength,
            ["outputFormat"] = outputFormat
        };

        return await ExecutePromptAsync(template, args, "KeyPoints");
    }

    [KernelFunction("AnalyzeContentNature")]
    [Description("Analyzes the input text and returns the most suitable summary focus category.")]
    public async Task<string> AnalyzeContentNatureAsync(string inputText)
    {
        _logger.LogInformation("Content analyzer invoked for automatic focus detection.");
        _logger.LogInformation("Analyzing semantic nature of incoming text...");

        var template = await _promptLoader.LoadAsync("content-analyzer");

        var args = new KernelArguments
        {
            ["inputText"] = inputText
        };

        var settings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.0,
            TopP = 0.5,
            MaxTokens = 20
        };

        args.ExecutionSettings = new Dictionary<string, PromptExecutionSettings>
        {
            ["default"] = settings
        };

        var result = await _kernel.InvokePromptAsync(template, args);

        _logger.LogInformation("Raw analyzer model response: {AnalyzerResponse}", result.GetValue<string>());

        var detectedFocus = result.GetValue<string>()?.Trim() ?? "General";

        _logger.LogInformation("Detected Summary Focus from Analyzer: {Focus}", detectedFocus);

        return detectedFocus;
    }

    private async Task<string> ExecutePromptAsync(string template, KernelArguments args, string summaryType)
    {
        _logger.LogInformation("Executing {SummaryType} summarization plugin.", summaryType);

        var settings = new OpenAIPromptExecutionSettings
        {
            Temperature = 0.1,
            TopP = 0.8
        };

        args.ExecutionSettings = new Dictionary<string, PromptExecutionSettings>
        {
            ["default"] = settings
        };

        var result = await _kernel.InvokePromptAsync(template, args);

        _logger.LogInformation("Raw response from kernel: {Response}", result.GetValue<string>());

        _logger.LogInformation("Summary generated successfully.");

        return result.GetValue<string>() ?? string.Empty;
    }
}