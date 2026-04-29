using AdaptiveSummaryGenerator.Core.Interfaces;
using AdaptiveSummaryGenerator.Core.Plugins;
using AdaptiveSummaryGenerator.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

namespace AdaptiveSummaryGenerator.Core.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPromptLoaderService, PromptLoaderService>();
        services.AddScoped<IKernelService, KernelService>();
        services.AddScoped<IRequestValidationService, RequestValidationService>();
        services.AddScoped<SummaryPlugin>();

        services.AddScoped<Kernel>(sp =>
        {
            var kernelBuilder = Kernel.CreateBuilder();

            kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: configuration["AzureOpenAI:DeploymentName"]!,
                endpoint: configuration["AzureOpenAI:Endpoint"]!,
                apiKey: configuration["AzureOpenAI:ApiKey"]!
            );

            return kernelBuilder.Build();
        });

        return services;
    }
}