using MinhaApi.Dominio.Enums;

namespace MinhaApi.Dominio.DTOs.Produtos;

public sealed record CreateProdutoRequest
{
    public required string Nome { get; init; }

    public required CategoriaProduto Categoria { get; init; }

    public required string CodigoBarras { get; init; }

    public required decimal Preco { get; init; }

    public required string NumeroSerie { get; init; }

    public required int Estoque { get; init; }
}
