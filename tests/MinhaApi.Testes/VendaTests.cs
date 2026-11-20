using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Enums;
using MinhaApi.Dominio.Services;

namespace MinhaApi.Testes;

public class VendaTests
{
    private static Cliente CriarClientePadrao() =>
        new("Carlos Almeida", "52998224725", "(35) 99999-1111", "carlos@email.com", "Rua A, 100 - Alfenas/MG");

    [Fact]
    public void RegistrarVenda_ComMultiplosItensEDescontoValido_DeveCalcularTotalEBaixarEstoque()
    {
        // Arrange
        var vendaService = new VendaService();
        var cliente = CriarClientePadrao();
        var filtroOleo = new Produto("Filtro de Óleo", CategoriaProduto.Motor, "78910001", 50.00m, "GOL-G5-2012", 10);
        var pastilhaFreio = new Produto("Pastilha de Freio", CategoriaProduto.Freios, "78910002", 120.00m, "GOL-G5-2012", 5);

        var itens = new List<(Produto, int)>
        {
            (filtroOleo, 2),     // 2 x 50 = 100
            (pastilhaFreio, 1)   // 1 x 120 = 120 -> Subtotal = 220
        };

        // Act
        var venda = vendaService.RegistrarVenda(cliente, itens, FormaPagamento.Pix, desconto: 20.00m);

        // Assert
        Assert.Equal(220.00m, venda.Subtotal);
        Assert.Equal(200.00m, venda.ValorTotal);
        Assert.Equal(FormaPagamento.Pix, venda.FormaPagamento);
        Assert.Equal(8, filtroOleo.Estoque);
        Assert.Equal(4, pastilhaFreio.Estoque);
    }

    [Fact]
    public void RegistrarVenda_MesmoClienteVariasCompras_DevePermitirERegistrarTodas()
    {
        // Arrange
        var vendaService = new VendaService();
        var cliente = CriarClientePadrao();
        var amortecedor = new Produto("Amortecedor Dianteiro", CategoriaProduto.Suspensao, "78920001", 300.00m, "CIVIC-G10", 6);

        // Act
        var primeiraVenda = vendaService.RegistrarVenda(cliente, new[] { (amortecedor, 2) }, FormaPagamento.CartaoCredito);
        var segundaVenda = vendaService.RegistrarVenda(cliente, new[] { (amortecedor, 1) }, FormaPagamento.Dinheiro);

        // Assert
        var comprasDoCliente = vendaService.ListarPorCliente(cliente.Id);
        Assert.NotEqual(primeiraVenda.Id, segundaVenda.Id);
        Assert.Equal(2, comprasDoCliente.Count);
        Assert.Equal(3, amortecedor.Estoque);
    }

    [Theory]
    [InlineData(-10.00)]
    [InlineData(100.01)]
    public void RegistrarVenda_ComDescontoNegativoOuMaiorQueSubtotal_DeveLancarArgumentException(decimal descontoInvalido)
    {
        // Arrange
        var vendaService = new VendaService();
        var cliente = CriarClientePadrao();
        var oleoMotor = new Produto("Óleo Sintético 5W30", CategoriaProduto.Fluidos, "78930001", 100.00m, "UNIVERSAL", 10);

        // Act & Assert
        var excecao = Assert.Throws<ArgumentException>(() =>
            vendaService.RegistrarVenda(cliente, new[] { (oleoMotor, 1) }, FormaPagamento.CartaoDebito, descontoInvalido));

        Assert.Contains("desconto", excecao.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(10, oleoMotor.Estoque); // Garante que não baixou estoque
    }

    [Fact]
    public void RegistrarVenda_ComEstoqueInsuficiente_DeveLancarInvalidOperationExceptionEManterEstoqueIntacto()
    {
        // Arrange
        var vendaService = new VendaService();
        var cliente = CriarClientePadrao();
        var velaIgnicao = new Produto("Vela de Ignição", CategoriaProduto.Eletrica, "78940001", 40.00m, "ONIX-2020", 10);
        var bateria = new Produto("Bateria 60Ah", CategoriaProduto.Eletrica, "78940002", 450.00m, "ONIX-2020", 2);

        var itens = new List<(Produto, int)>
        {
            (velaIgnicao, 4), // Tem estoque (10)
            (bateria, 3)      // Estoque insuficiente (só tem 2)
        };

        // Act & Assert
        var excecao = Assert.Throws<InvalidOperationException>(() =>
            vendaService.RegistrarVenda(cliente, itens, FormaPagamento.Pix));

        Assert.Contains("Estoque insuficiente", excecao.Message);
        Assert.Equal(10, velaIgnicao.Estoque); // Primeiro item não pode ter sofrido baixa
        Assert.Equal(2, bateria.Estoque);
    }

    [Fact]
    public void RegistrarVenda_SemItensOuSemCliente_DeveLancarArgumentException()
    {
        // Arrange
        var vendaService = new VendaService();
        var cliente = CriarClientePadrao();
        var tapete = new Produto("Jogo de Tapetes", CategoriaProduto.Acessorios, "78950001", 90.00m, "HB20-2021", 5);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            vendaService.RegistrarVenda(cliente, Array.Empty<(Produto, int)>(), FormaPagamento.Dinheiro));

        Assert.Throws<ArgumentException>(() =>
            vendaService.RegistrarVenda(null!, new[] { (tapete, 1) }, FormaPagamento.Dinheiro));
    }
}