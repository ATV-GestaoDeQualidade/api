using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Enums;

namespace MinhaApi.Dominio.Services;

public interface IProdutoService
{
    Task<Produto> CadastrarAsync(
        string nome,
        CategoriaProduto categoria,
        string codigoBarras,
        decimal preco,
        string numeroSerie,
        int estoque,
        CancellationToken cancellationToken);

    Task<Produto?> BuscarPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Produto>> ListarTodosAsync(CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Produto>> FiltrarAsync(
        CategoriaProduto? categoria,
        string? numeroSerie,
        CancellationToken cancellationToken);
}
