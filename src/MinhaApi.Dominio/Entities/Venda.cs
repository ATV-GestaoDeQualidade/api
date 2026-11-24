using MinhaApi.Dominio.Enums;

namespace MinhaApi.Dominio.Entities;

public class Venda
{
    private readonly List<ItemVenda> _itens;

    public Guid Id { get; private set; }
    public Cliente Cliente { get; private set; } = null!;
    public DateTime DataVenda { get; private set; }
    public FormaPagamento FormaPagamento { get; private set; }
    public decimal Desconto { get; private set; }
    public IReadOnlyCollection<ItemVenda> Itens => _itens.AsReadOnly();

    public decimal Subtotal => _itens.Sum(i => i.Subtotal);
    public decimal ValorTotal => Subtotal - Desconto;

    private Venda()
    {
        _itens = new List<ItemVenda>();
    }

    public Venda(Cliente cliente, IEnumerable<ItemVenda> itens, FormaPagamento formaPagamento, decimal desconto = 0m)
    {
        if (cliente is null)
            throw new ArgumentException("A venda deve estar vinculada a um cliente cadastrado.", nameof(cliente));

        var listaItens = itens?.ToList() ?? new List<ItemVenda>();
        if (listaItens.Count == 0)
            throw new ArgumentException("A venda deve possuir pelo menos um produto.", nameof(itens));

        if (!Enum.IsDefined(typeof(FormaPagamento), formaPagamento))
            throw new ArgumentException("Forma de pagamento inválida.", nameof(formaPagamento));

        if (desconto < 0)
            throw new ArgumentException("O valor do desconto não pode ser negativo.", nameof(desconto));

        var subtotalCalculado = listaItens.Sum(i => i.Subtotal);
        if (desconto > subtotalCalculado)
            throw new ArgumentException("O desconto não pode ser maior que o subtotal da venda.", nameof(desconto));

        Id = Guid.NewGuid();
        Cliente = cliente;
        DataVenda = DateTime.UtcNow;
        FormaPagamento = formaPagamento;
        Desconto = desconto;
        _itens = listaItens;
    }
}