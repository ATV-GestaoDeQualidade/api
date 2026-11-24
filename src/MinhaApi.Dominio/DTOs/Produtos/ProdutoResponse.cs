using MinhaApi.Dominio.Enums;

namespace MinhaApi.Dominio.DTOs.Produtos;

public sealed record ProdutoResponse(
    Guid Id,
    string Nome,
    CategoriaProduto Categoria,
    string CodigoBarras,
    decimal Preco,
    string NumeroSerie,
    int Estoque);
