namespace MinhaApi.Dominio.Entities;

public class ItemVenda
{
    public Produto Produto { get; private set; }
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Subtotal => PrecoUnitario * Quantidade;

    public ItemVenda(Produto produto, int quantidade)
    {
        if (produto is null)
            throw new ArgumentException("O produto do item não pode ser nulo.", nameof(produto));

        if (quantidade <= 0)
            throw new ArgumentException("A quantidade vendida deve ser maior que zero.", nameof(quantidade));

        Produto = produto;
        Quantidade = quantidade;
        PrecoUnitario = produto.Preco;
    }
}