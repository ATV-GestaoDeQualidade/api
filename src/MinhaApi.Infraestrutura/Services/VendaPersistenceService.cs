using Microsoft.EntityFrameworkCore;
using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Enums;
using MinhaApi.Dominio.Services;
using MinhaApi.Infraestrutura.Data;

namespace MinhaApi.Infraestrutura.Services;

public sealed class VendaPersistenceService(MinhaApiDbContext dbContext) : IVendaService
{
    public async Task<Venda> RegistrarVendaAsync(
        Cliente cliente,
        IEnumerable<(Produto produto, int quantidade)> itensSolicitados,
        FormaPagamento formaPagamento,
        decimal desconto,
        CancellationToken cancellationToken)
    {
        if (cliente is null)
            throw new ArgumentException("A venda deve estar vinculada a um cliente cadastrado.", nameof(cliente));

        if (itensSolicitados is null)
            throw new ArgumentException("A lista de itens da venda é obrigatória.", nameof(itensSolicitados));

        var listaBruta = itensSolicitados.ToList();
        if (listaBruta.Count == 0)
            throw new ArgumentException("A venda deve possuir pelo menos um produto.", nameof(itensSolicitados));

        if (listaBruta.Any(item => item.produto is null))
            throw new ArgumentException("Todos os itens devem possuir um produto válido.", nameof(itensSolicitados));

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var clientePersistido = await dbContext.Clientes
            .FirstOrDefaultAsync(item => item.Id == cliente.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Cliente não encontrado.");

        var itensAgrupados = listaBruta
            .GroupBy(item => item.produto.Id)
            .Select(grupo => new
            {
                ProdutoId = grupo.Key,
                Quantidade = grupo.Sum(item => item.quantidade)
            })
            .ToArray();

        var produtoIds = itensAgrupados.Select(item => item.ProdutoId).ToArray();
        var produtos = await dbContext.Produtos
            .Where(produto => produtoIds.Contains(produto.Id))
            .ToDictionaryAsync(produto => produto.Id, cancellationToken);

        if (produtos.Count != produtoIds.Length)
            throw new KeyNotFoundException("Um ou mais produtos não foram encontrados.");

        foreach (var item in itensAgrupados)
        {
            if (item.Quantidade <= 0)
                throw new ArgumentException(
                    $"A quantidade do produto '{produtos[item.ProdutoId].Nome}' deve ser maior que zero.");

            if (item.Quantidade > produtos[item.ProdutoId].Estoque)
                throw new InvalidOperationException(
                    $"Estoque insuficiente para o produto '{produtos[item.ProdutoId].Nome}'. Disponível: {produtos[item.ProdutoId].Estoque}.");
        }

        var itensVenda = itensAgrupados
            .Select(item => new ItemVenda(produtos[item.ProdutoId], item.Quantidade))
            .ToArray();
        var venda = new Venda(clientePersistido, itensVenda, formaPagamento, desconto);

        foreach (var item in itensAgrupados)
        {
            produtos[item.ProdutoId].BaixarEstoque(item.Quantidade);
        }

        dbContext.Vendas.Add(venda);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return venda;
    }

    public Task<Venda?> BuscarPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        ConsultaCompleta()
            .AsNoTracking()
            .FirstOrDefaultAsync(venda => venda.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Venda>> ListarPorClienteAsync(
        Guid clienteId,
        CancellationToken cancellationToken) =>
        await ConsultaCompleta()
            .AsNoTracking()
            .Where(venda => EF.Property<Guid>(venda, "ClienteId") == clienteId)
            .ToArrayAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Venda>> ListarTodasAsync(CancellationToken cancellationToken) =>
        await ConsultaCompleta().AsNoTracking().ToArrayAsync(cancellationToken);

    private IQueryable<Venda> ConsultaCompleta() =>
        dbContext.Vendas
            .Include(venda => venda.Cliente)
            .Include(venda => venda.Itens)
            .ThenInclude(item => item.Produto);
}
