using AdaptiveSummaryGenerator.Api.Extensions;
using AdaptiveSummaryGenerator.Core.DependencyInjection;
using AdaptiveSummaryGenerator.Core.Enums;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AdaptiveSummaryGenerator.Api",
        Version = "v1",
        Description = "Adaptive Summary Generator API using Semantic Kernel Function Calling"
    });

    c.MapType<SummaryLengthType>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(SummaryLengthType))
            .Select(x => new OpenApiString(x))
            .Cast<IOpenApiAny>()
            .ToList()
    });

    c.MapType<SummaryFocusType>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(SummaryFocusType))
            .Select(x => new OpenApiString(x))
            .Cast<IOpenApiAny>()
            .ToList()
    });

    c.MapType<OutputFormatType>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(OutputFormatType))
            .Select(x => new OpenApiString(x))
            .Cast<IOpenApiAny>()
            .ToList()
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapApplicationEndpoints();
app.Run();