using MinhaApi.Dominio.Enums;

namespace MinhaApi.Dominio.DTOs.Vendas;

public sealed record VendaResponse(
    Guid Id,
    Guid ClienteId,
    string NomeCliente,
    DateTimeOffset DataVenda,
    FormaPagamento FormaPagamento,
    decimal Desconto,
    IReadOnlyCollection<ItemVendaResponse> Itens,
    decimal Subtotal,
    decimal ValorTotal);
