using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Enums;

namespace MinhaApi.Dominio.Services;

public sealed class ProdutoService
{
    private readonly List<Produto> _produtos = new();
    private readonly object _lock = new();

    public Produto Cadastrar(
        string nome,
        CategoriaProduto categoria,
        string codigoBarras,
        decimal preco,
        string numeroSerie,
        int estoque)
    {
        lock (_lock)
        {
            if (_produtos.Any(p => string.Equals(
                    p.CodigoBarras,
                    codigoBarras?.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Já existe um produto cadastrado com este código de barras.");
            }

            var produto = new Produto(nome, categoria, codigoBarras, preco, numeroSerie, estoque);
            _produtos.Add(produto);
            return produto;
        }
    }

    public Produto? BuscarPorId(Guid id)
    {
        lock (_lock)
        {
            return _produtos.FirstOrDefault(p => p.Id == id);
        }
    }

    public IReadOnlyCollection<Produto> ListarTodos()
    {
        lock (_lock)
        {
            return _produtos.ToArray();
        }
    }
}
