using MinhaApi.Dominio.Enums;

namespace MinhaApi.Dominio.DTOs.Vendas;

public sealed record CreateVendaRequest
{
    public required Guid ClienteId { get; init; }
    public required IReadOnlyCollection<CreateItemVendaRequest> Itens { get; init; }
    public required FormaPagamento FormaPagamento { get; init; }
    public decimal Desconto { get; init; }
}
