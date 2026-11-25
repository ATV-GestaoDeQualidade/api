using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Enums;
using MinhaApi.Dominio.Services;

namespace MinhaApi.Testes;

public class ProdutoTests
{
    [Fact]
    public void Cadastrar_ComDadosValidos_DeveAdicionarProdutoComEstoqueInicial()
    {
        var service = new ProdutoService();

        var produto = service.Cadastrar(
            "Pastilha de Freio",
            CategoriaProduto.Freios,
            "78910001",
            120.00m,
            "GOL-G5-2012",
            8);

        Assert.Equal("Pastilha de Freio", produto.Nome);
        Assert.Equal("78910001", produto.CodigoBarras);
        Assert.Equal(120.00m, produto.Preco);
        Assert.Equal("GOL-G5-2012", produto.NumeroSerie);
        Assert.Equal(8, produto.Estoque);
        Assert.Single(service.ListarTodos());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Cadastrar_ComPrecoMenorOuIgualAZero_DeveRejeitarCadastro(decimal preco)
    {
        var service = new ProdutoService();

        Assert.Throws<ArgumentException>(() =>
            service.Cadastrar("Pastilha de Freio", CategoriaProduto.Freios, "78910001", preco, "GOL-G5-2012", 8));

        Assert.Empty(service.ListarTodos());
    }

    [Fact]
    public void Cadastrar_ComEstoqueNegativo_DeveRejeitarCadastro()
    {
        var service = new ProdutoService();

        Assert.Throws<ArgumentException>(() =>
            service.Cadastrar("Pastilha de Freio", CategoriaProduto.Freios, "78910001", 120.00m, "GOL-G5-2012", -1));

        Assert.Empty(service.ListarTodos());
    }

    [Fact]
    public void Cadastrar_ComNomeCodigoDeBarrasOuNumeroDeSerieVazio_DeveRejeitarCadastro()
    {
        var service = new ProdutoService();

        Assert.Throws<ArgumentException>(() =>
            service.Cadastrar("", CategoriaProduto.Freios, "78910001", 120.00m, "GOL-G5-2012", 8));
        Assert.Throws<ArgumentException>(() =>
            service.Cadastrar("Pastilha de Freio", CategoriaProduto.Freios, " ", 120.00m, "GOL-G5-2012", 8));
        Assert.Throws<ArgumentException>(() =>
            service.Cadastrar("Pastilha de Freio", CategoriaProduto.Freios, "78910001", 120.00m, "", 8));

        Assert.Empty(service.ListarTodos());
    }

    [Fact]
    public void Cadastrar_ComCodigoDeBarrasDuplicado_DeveRejeitarCadastro()
    {
        var service = new ProdutoService();
        service.Cadastrar("Pastilha de Freio", CategoriaProduto.Freios, "78910001", 120.00m, "GOL-G5-2012", 8);

        var excecao = Assert.Throws<InvalidOperationException>(() =>
            service.Cadastrar("Pastilha Dianteira", CategoriaProduto.Freios, " 78910001 ", 140.00m, "GOL-G5-2012", 4));

        Assert.Contains("código de barras", excecao.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Single(service.ListarTodos());
    }

    [Fact]
    public void Cadastrar_ComEstoqueInicialZerado_DevePermitirCadastroMasNaoVenda()
    {
        var produtoService = new ProdutoService();
        var produto = produtoService.Cadastrar(
            "Pastilha de Freio",
            CategoriaProduto.Freios,
            "78910001",
            120.00m,
            "GOL-G5-2012",
            0);
        var vendaService = new VendaService();
        var cliente = new Cliente("Carlos Almeida", "52998224725", "(35) 99999-1111", "carlos@email.com", "Rua A, 100");

        Assert.Equal(0, produto.Estoque);
        Assert.Throws<InvalidOperationException>(() =>
            vendaService.RegistrarVenda(cliente, new[] { (produto, 1) }, FormaPagamento.Pix));
        Assert.Equal(0, produto.Estoque);
    }

    [Fact]
    public void Cadastrar_ComMesmoNumeroDeSerie_DevePermitirProdutosDiferentes()
    {
        var service = new ProdutoService();

        service.Cadastrar("Pastilha de Freio Marca A", CategoriaProduto.Freios, "78910001", 120.00m, "GOL-G5-2012", 8);
        service.Cadastrar("Pastilha de Freio Marca B", CategoriaProduto.Freios, "78910002", 135.00m, "GOL-G5-2012", 5);

        Assert.Equal(2, service.ListarTodos().Count);
    }
}
