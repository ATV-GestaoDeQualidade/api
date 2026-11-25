namespace MinhaApi.Dominio.DTOs.Vendas;

public sealed record ItemVendaResponse(
    Guid ProdutoId,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal);
