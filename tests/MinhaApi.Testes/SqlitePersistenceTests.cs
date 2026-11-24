using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Enums;
using MinhaApi.Infraestrutura.Data;
using MinhaApi.Infraestrutura.Services;

namespace MinhaApi.Testes;

public class SqlitePersistenceTests
{
    [Fact]
    public async Task RegistrarVenda_DevePersistirClienteProdutoVendaEBaixaDeEstoque()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<MinhaApiDbContext>()
            .UseSqlite(connection)
            .Options;

        Guid clienteId;
        Guid produtoId;
        Guid vendaId;

        await using (var dbContext = new MinhaApiDbContext(options))
        {
            await dbContext.Database.MigrateAsync();
            var clienteService = new ClientePersistenceService(dbContext);
            var produtoService = new ProdutoPersistenceService(dbContext);
            var vendaService = new VendaPersistenceService(dbContext);

            var cliente = await clienteService.CadastrarAsync(
                "Carlos Almeida",
                "52998224725",
                "(35) 99999-1111",
                "carlos@email.com",
                "Rua A, 100",
                CancellationToken.None);
            var produto = await produtoService.CadastrarAsync(
                "Pastilha de Freio",
                CategoriaProduto.Freios,
                "78910001",
                120.00m,
                "GOL-G5-2012",
                8,
                CancellationToken.None);
            var venda = await vendaService.RegistrarVendaAsync(
                cliente,
                new[] { (produto, 2) },
                FormaPagamento.Pix,
                10.00m,
                CancellationToken.None);

            clienteId = cliente.Id;
            produtoId = produto.Id;
            vendaId = venda.Id;
        }

        await using var readContext = new MinhaApiDbContext(options);
        var clientePersistido = await readContext.Clientes
            .AsNoTracking()
            .SingleAsync(cliente => cliente.Id == clienteId);
        var produtoPersistido = await readContext.Produtos
            .AsNoTracking()
            .SingleAsync(produto => produto.Id == produtoId);
        var vendaPersistida = await readContext.Vendas
            .AsNoTracking()
            .Include(venda => venda.Cliente)
            .Include(venda => venda.Itens)
            .ThenInclude(item => item.Produto)
            .SingleAsync(venda => venda.Id == vendaId);

        Assert.Equal("Carlos Almeida", clientePersistido.Nome);
        Assert.Equal(6, produtoPersistido.Estoque);
        Assert.Equal(240.00m, vendaPersistida.Subtotal);
        Assert.Equal(230.00m, vendaPersistida.ValorTotal);
        Assert.Equal("Pastilha de Freio", Assert.Single(vendaPersistida.Itens).Produto.Nome);
    }

    [Fact]
    public async Task CadastrarProduto_ComCodigoDeBarrasDuplicado_DeveRejeitarCadastro()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<MinhaApiDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var dbContext = new MinhaApiDbContext(options);
        await dbContext.Database.MigrateAsync();
        var service = new ProdutoPersistenceService(dbContext);

        await service.CadastrarAsync(
            "Pastilha de Freio",
            CategoriaProduto.Freios,
            "78910001",
            120.00m,
            "GOL-G5-2012",
            8,
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CadastrarAsync(
                "Pastilha Dianteira",
                CategoriaProduto.Freios,
                " 78910001 ",
                140.00m,
                "GOL-G5-2012",
                4,
                CancellationToken.None));

        Assert.Equal(1, await dbContext.Produtos.CountAsync());
    }

    [Fact]
    public async Task RegistrarVenda_ComEstoqueInsuficiente_DeveManterEstoqueESemVenda()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<MinhaApiDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var dbContext = new MinhaApiDbContext(options);
        await dbContext.Database.MigrateAsync();

        var clienteService = new ClientePersistenceService(dbContext);
        var produtoService = new ProdutoPersistenceService(dbContext);
        var vendaService = new VendaPersistenceService(dbContext);
        var cliente = await clienteService.CadastrarAsync(
            "Carlos Almeida",
            "52998224725",
            "(35) 99999-1111",
            "carlos@email.com",
            "Rua A, 100",
            CancellationToken.None);
        var produto = await produtoService.CadastrarAsync(
            "Pastilha de Freio",
            CategoriaProduto.Freios,
            "78910001",
            120.00m,
            "GOL-G5-2012",
            1,
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            vendaService.RegistrarVendaAsync(
                cliente,
                new[] { (produto, 2) },
                FormaPagamento.Pix,
                0m,
                CancellationToken.None));

        Assert.Equal(1, await dbContext.Produtos
            .AsNoTracking()
            .Select(item => item.Estoque)
            .SingleAsync());
        Assert.Empty(await dbContext.Vendas.ToArrayAsync());
    }
}
