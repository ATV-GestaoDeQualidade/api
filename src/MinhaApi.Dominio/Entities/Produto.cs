using MinhaApi.Dominio.Enums;

namespace MinhaApi.Dominio.Entities;

public class Produto
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public CategoriaProduto Categoria { get; private set; }
    public string CodigoBarras { get; private set; }
    public decimal Preco { get; private set; }
    public string NumeroSerie { get; private set; }
    public int Estoque { get; private set; }

    public Produto(
        string nome,
        CategoriaProduto categoria,
        string codigoBarras,
        decimal preco,
        string numeroSerie,
        int estoque)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do produto é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(codigoBarras))
            throw new ArgumentException("O código de barras é obrigatório.", nameof(codigoBarras));

        if (preco <= 0)
            throw new ArgumentException("O preço do produto deve ser maior que zero.", nameof(preco));

        if (string.IsNullOrWhiteSpace(numeroSerie))
            throw new ArgumentException("O número de série do veículo é obrigatório.", nameof(numeroSerie));

        if (estoque < 0)
            throw new ArgumentException("O estoque inicial não pode ser negativo.", nameof(estoque));

        Id = Guid.NewGuid();
        Nome = nome.Trim();
        Categoria = categoria;
        CodigoBarras = codigoBarras.Trim();
        Preco = preco;
        NumeroSerie = numeroSerie.Trim().ToUpperInvariant();
        Estoque = estoque;
    }

    public void BaixarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("A quantidade para baixa deve ser maior que zero.", nameof(quantidade));

        if (quantidade > Estoque)
            throw new InvalidOperationException($"Estoque insuficiente para o produto '{Nome}'. Disponível: {Estoque}.");

        Estoque -= quantidade;
    }
}