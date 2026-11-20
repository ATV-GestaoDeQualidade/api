using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Enums;

namespace MinhaApi.Dominio.Services;

public class VendaService
{
    private readonly List<Venda> _vendas = new();

    public Venda RegistrarVenda(
        Cliente cliente,
        IEnumerable<(Produto produto, int quantidade)> itensSolicitados,
        FormaPagamento formaPagamento,
        decimal desconto = 0m)
    {
        if (itensSolicitados is null)
            throw new ArgumentException("A lista de itens da venda é obrigatória.", nameof(itensSolicitados));

        var listaBruta = itensSolicitados.ToList();
        if (listaBruta.Count == 0)
            throw new ArgumentException("A venda deve possuir pelo menos um produto.", nameof(itensSolicitados));

        if (listaBruta.Any(i => i.produto is null))
            throw new ArgumentException("Todos os itens devem possuir um produto válido.", nameof(itensSolicitados));

        // Agrupa caso o mesmo produto seja adicionado mais de uma vez na mesma venda
        var itensAgrupados = listaBruta
            .GroupBy(i => i.produto)
            .Select(g => (Produto: g.Key, QuantidadeTotal: g.Sum(x => x.quantidade)))
            .ToList();

        // 1. Valida quantidades e disponibilidade em estoque antes de alterar qualquer saldo
        foreach (var item in itensAgrupados)
        {
            if (item.QuantidadeTotal <= 0)
                throw new ArgumentException($"A quantidade do produto '{item.Produto.Nome}' deve ser maior que zero.");

            if (item.QuantidadeTotal > item.Produto.Estoque)
                throw new InvalidOperationException($"Estoque insuficiente para o produto '{item.Produto.Nome}'. Disponível: {item.Produto.Estoque}.");
        }

        // 2. Cria a venda validando cliente, forma de pagamento e limite de desconto
        var itensVenda = itensAgrupados
            .Select(i => new ItemVenda(i.Produto, i.QuantidadeTotal))
            .ToList();

        var venda = new Venda(cliente, itensVenda, formaPagamento, desconto);

        // 3. Efetiva a baixa no estoque de cada produto vendido
        foreach (var item in itensAgrupados)
        {
            item.Produto.BaixarEstoque(item.QuantidadeTotal);
        }

        _vendas.Add(venda);
        return venda;
    }

    public IReadOnlyCollection<Venda> ListarPorCliente(Guid clienteId)
        => _vendas.Where(v => v.Cliente.Id == clienteId).ToList().AsReadOnly();

    public IReadOnlyCollection<Venda> ListarTodas() => _vendas.AsReadOnly();
}