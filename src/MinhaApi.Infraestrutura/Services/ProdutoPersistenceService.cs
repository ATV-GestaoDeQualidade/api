using Microsoft.EntityFrameworkCore;
using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Enums;
using MinhaApi.Dominio.Services;
using MinhaApi.Infraestrutura.Data;

namespace MinhaApi.Infraestrutura.Services;

public sealed class ProdutoPersistenceService(MinhaApiDbContext dbContext) : IProdutoService
{
    public async Task<Produto> CadastrarAsync(
        string nome,
        CategoriaProduto categoria,
        string codigoBarras,
        decimal preco,
        string numeroSerie,
        int estoque,
        CancellationToken cancellationToken)
    {
        var codigoNormalizado = codigoBarras.Trim();
        if (await dbContext.Produtos.AnyAsync(
                produto => produto.CodigoBarras == codigoNormalizado,
                cancellationToken))
        {
            throw new InvalidOperationException("Já existe um produto cadastrado com este código de barras.");
        }

        var produto = new Produto(nome, categoria, codigoBarras, preco, numeroSerie, estoque);
        dbContext.Produtos.Add(produto);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqliteConstraintErrors.IsUniqueConstraint(exception))
        {
            dbContext.Entry(produto).State = EntityState.Detached;
            throw new InvalidOperationException("Já existe um produto cadastrado com este código de barras.", exception);
        }

        return produto;
    }

    public Task<Produto?> BuscarPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Produtos.FirstOrDefaultAsync(produto => produto.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Produto>> ListarTodosAsync(CancellationToken cancellationToken) =>
        await dbContext.Produtos.AsNoTracking().ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Produto>> FiltrarAsync(
        CategoriaProduto? categoria,
        string? numeroSerie,
        CancellationToken cancellationToken)
    {
        var numeroSerieNormalizado = string.IsNullOrWhiteSpace(numeroSerie)
            ? null
            : numeroSerie.Trim().ToUpperInvariant();

        if (!categoria.HasValue && numeroSerieNormalizado is null)
            throw new ArgumentException("Informe ao menos uma categoria ou número de série.");

        if (categoria.HasValue && !Enum.IsDefined(categoria.Value))
            throw new ArgumentException("A categoria informada é inválida.", nameof(categoria));

        var consulta = dbContext.Produtos.AsNoTracking().AsQueryable();

        if (categoria.HasValue)
            consulta = consulta.Where(produto => produto.Categoria == categoria.Value);

        if (numeroSerieNormalizado is not null)
            consulta = consulta.Where(produto => produto.NumeroSerie == numeroSerieNormalizado);

        return await consulta
            .OrderBy(produto => produto.Nome)
            .ToArrayAsync(cancellationToken);
    }
}
