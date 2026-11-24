using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Enums;

namespace MinhaApi.Dominio.Services;

public interface IProdutoService
{
    Produto Cadastrar(
        string nome,
        CategoriaProduto categoria,
        string codigoBarras,
        decimal preco,
        string numeroSerie,
        int estoque);

    Produto? BuscarPorId(Guid id);

    IReadOnlyCollection<Produto> ListarTodos();
}
