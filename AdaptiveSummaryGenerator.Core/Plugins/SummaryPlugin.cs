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

        return await ExecutePromptAsync(template, args);
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

        return await ExecutePromptAsync(template, args);
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

        return await ExecutePromptAsync(template, args);
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

        return await ExecutePromptAsync(template, args);
    }

    private async Task<string> ExecutePromptAsync(string template, KernelArguments args)
    {
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