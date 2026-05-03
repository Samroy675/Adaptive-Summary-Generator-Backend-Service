using AdaptiveSummaryGenerator.UI.Models;

namespace AdaptiveSummaryGenerator.UI.Services;

// ─── DUMMY SERVICE ────────────────────────────────────────────────────────────
// Simulates backend responses locally. Replace with RealSummaryService later.
// To switch: in Program.cs change AddScoped<ISummaryService, DummySummaryService>
//            to         AddScoped<ISummaryService, RealSummaryService>
public class DummySummaryService : ISummaryService
{

    private static readonly Dictionary<string, List<string>> DummyData = new()
    {
        ["Technical-Short"] =
        [
            "The system employs a microservices architecture secured via JWT and uses REST APIs for inter-service communication.",
            "Built on microservices with REST APIs and Azure Blob Storage, the app ensures scalable, secure service interactions.",
        ],
        ["Technical-Medium"] =
        [
            "The application is built on a microservices architecture that facilitates modular and scalable service development. It employs REST APIs to enable communication between services, ensuring interoperability and stateless interactions. Dependency injection manages service dependencies efficiently, promoting loose coupling and easier testing. Azure Blob Storage provides scalable cloud-based storage for handling large amounts of unstructured data. Security is enforced through JWT-based authentication ensuring secure and stateless communication.",
            "At its core, the application uses a microservices design pattern to isolate business domains and improve resilience. REST APIs serve as the communication backbone, while Azure Blob Storage handles unstructured data at scale. JWT tokens authenticate service-to-service calls, maintaining a stateless security model. Dependency injection keeps components decoupled and highly testable throughout the system.",
        ],
        ["Technical-Detailed"] =
        [
            "The application is architected around a microservices pattern where each service owns its bounded context and exposes REST API endpoints for inter-service communication. This ensures statelessness and horizontal scalability. Dependency injection is applied throughout to maintain loose coupling and support testability across all service layers. Azure Blob Storage handles large volumes of unstructured data with durable, geo-redundant object storage. Authentication and authorization use JWT tokens validated at service boundaries to enforce zero-trust security. This combination results in a robust, cloud-native system built for scale.",
        ],
        ["Business-Short"] =
        [
            "The platform delivers a secure, scalable solution that accelerates service delivery and reduces operational overhead.",
            "Designed for reliability and growth, the solution ensures secure operations and efficient data management.",
        ],
        ["Business-Medium"] =
        [
            "The application delivers high reliability and operational scalability, enabling the business to grow without infrastructure constraints. Its modular architecture allows teams to deploy and iterate on features independently, reducing time-to-market. Cloud storage supports large-scale data handling while strong authentication safeguards customer data.",
            "The platform offers a scalable and maintainable foundation that supports rapid feature delivery. Independent service deployments allow teams to work in parallel, reducing bottlenecks. The cloud-first storage strategy minimises infrastructure costs while ensuring data availability.",
        ],
        ["Business-Detailed"] =
        [
            "The application represents a strategic investment in scalable, cloud-native infrastructure that directly supports business growth. By decomposing the platform into independently deployable services, engineering teams deliver features faster with fewer cross-team dependencies. Azure Blob Storage handles large business data volumes cost-effectively with high availability. JWT-based security protects customer data and ensures regulatory compliance, critical for enterprise clients. The architecture is aligned with long-term goals of operational resilience, developer productivity, and customer trust.",
        ],
        ["General-Short"] =
        [
            "The application uses modern cloud technologies to ensure secure, scalable, and efficient data handling.",
            "Built with proven technologies, the system is secure, modular, and capable of handling growing workloads.",
        ],
        ["General-Medium"] =
        [
            "The application leverages modern cloud infrastructure to deliver a secure and highly scalable service. Well-established patterns such as microservices and dependency injection keep the system modular and maintainable. Cloud storage handles large data volumes efficiently while authentication mechanisms protect all service access.",
        ],
        ["General-Detailed"] =
        [
            "The application brings together a set of modern, proven technologies to deliver a secure, scalable, and maintainable platform. A microservices architecture ensures each component can evolve independently, reducing risk during updates. REST APIs provide a standard communication layer between services, while Azure Blob Storage offers durable cloud-native persistence. JWT authentication secures every interaction, ensuring only authorised parties can access sensitive operations. The overall design is built to grow with the organisation.",
        ],
        ["KeyPoints-Short"] =
        [
            "• Microservices architecture\n• REST API communication\n• JWT authentication\n• Azure Blob Storage\n• Dependency injection",
            "• Modular microservices design\n• Stateless REST APIs\n• JWT-based security\n• Cloud-native storage\n• Loose-coupled components",
        ],
        ["KeyPoints-Medium"] =
        [
            "• Microservices architecture for modularity and independent deployments\n• REST APIs for stateless inter-service communication\n• Dependency injection promotes loose coupling and testability\n• Azure Blob Storage handles unstructured data at scale\n• JWT authentication ensures secure, stateless communication\n• Design supports scalability, maintainability, and security",
        ],
        ["KeyPoints-Detailed"] =
        [
            "• Microservices: each service owns its bounded context and is independently deployable\n• REST APIs: stateless HTTP endpoints enable reliable cross-service communication\n• Dependency Injection: loose coupling across all layers enables testing and maintainability\n• Azure Blob Storage: durable, geo-redundant object storage for unstructured data\n• JWT Authentication: token-based security validates identity at every service boundary\n• Scalability: horizontal scaling supported by stateless services and cloud infrastructure\n• Maintainability: modular design lets teams iterate independently with minimal risk",
        ],
    };

    private static readonly Dictionary<string, string> FocusKeywords = new()
    {
        ["microservices"] = "Technical",
        ["rest api"] = "Technical",
        ["jwt"] = "Technical",
        ["blob storage"] = "Technical",
        ["dependency injection"] = "Technical",
        ["architecture"] = "Technical",
        ["revenue"] = "Business",
        ["productivity"] = "Business",
        ["customer"] = "Business",
        ["roi"] = "Business",
        ["strategy"] = "Business",
        ["profit"] = "Business",
    };

    public async Task<SummaryResponse> GenerateSummaryAsync(SummaryRequest request)
    {
        // Simulate API latency
        await Task.Delay(1400);

        string resolvedFocus = request.SummaryFocus == "Auto"
            ? DetectFocus(request.InputText)
            : request.SummaryFocus;

        string key = $"{resolvedFocus}-{request.SummaryLength}";
        if (!DummyData.ContainsKey(key)) key = "Technical-Medium";

        // Pick a random option from the list so Regenerate returns a fresh variation
        var options = DummyData[key];
        string summary = options[Random.Shared.Next(options.Count)];

        return new SummaryResponse
        {
            GeneratedSummary = summary,
            IsSuccess        = true,
            Message          = "Summary generated successfully.",
            DetectedFocus    = resolvedFocus,
            LengthUsed       = request.SummaryLength,
            FormatUsed       = request.OutputFormat,
            GeneratedAt      = DateTime.Now,
        };
    }

    private string DetectFocus(string text)
    {
        var lower  = text.ToLower();
        var scores = new Dictionary<string, int> { ["Technical"] = 0, ["Business"] = 0, ["General"] = 0 };
        foreach (var kv in FocusKeywords)
            if (lower.Contains(kv.Key)) scores[kv.Value]++;
        return scores.OrderByDescending(x => x.Value).First().Key;
    }
}
