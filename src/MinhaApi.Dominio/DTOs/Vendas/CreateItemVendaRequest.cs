namespace MinhaApi.Dominio.DTOs.Vendas;

public sealed record CreateItemVendaRequest
{
    public required Guid ProdutoId { get; init; }
    public required int Quantidade { get; init; }
}
