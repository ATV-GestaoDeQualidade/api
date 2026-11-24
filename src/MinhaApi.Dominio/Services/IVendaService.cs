using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Enums;

namespace MinhaApi.Dominio.Services;

public interface IVendaService
{
    Task<Venda> RegistrarVendaAsync(
        Cliente cliente,
        IEnumerable<(Produto produto, int quantidade)> itensSolicitados,
        FormaPagamento formaPagamento,
        decimal desconto,
        CancellationToken cancellationToken);

    Task<Venda?> BuscarPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Venda>> ListarPorClienteAsync(Guid clienteId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Venda>> ListarTodasAsync(CancellationToken cancellationToken);
}
