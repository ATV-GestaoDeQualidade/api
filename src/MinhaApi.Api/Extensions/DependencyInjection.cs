using System.Text.Json.Serialization;
using MinhaApi.Api.Middleware;
using MinhaApi.Dominio.Services;

namespace MinhaApi.Api.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks();
        services.AddSingleton<IProdutoService, ProdutoService>();
        services.AddExceptionHandler<ApiExceptionHandler>();
        services.AddProblemDetails();
        services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(allowIntegerValues: false)));

        return services;
    }
}
