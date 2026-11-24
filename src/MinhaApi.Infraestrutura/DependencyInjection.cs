using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinhaApi.Dominio.Services;
using MinhaApi.Infraestrutura.Data;
using MinhaApi.Infraestrutura.Services;

namespace MinhaApi.Infraestrutura;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("A conexão DefaultConnection não foi configurada.");

        services.AddDbContext<MinhaApiDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IClienteService, ClientePersistenceService>();
        services.AddScoped<IProdutoService, ProdutoPersistenceService>();
        services.AddScoped<IVendaService, VendaPersistenceService>();

        return services;
    }
}
