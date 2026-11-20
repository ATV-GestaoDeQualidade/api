using MinhaApi.Dominio.Services;

namespace MinhaApi.Testes;

public class ClienteTests
{
    [Fact]
    public void Cadastrar_ComDadosValidosPfEPj_DeveRegistrarClientesComSucesso()
    {
        // Arrange
        var service = new ClienteService();

        // Act
        var clientePf = service.Cadastrar(
            "Carlos Almeida",
            "52998224725",
            "(35) 99999-1111",
            "carlos@email.com",
            "Rua das Flores, 120 - Alfenas/MG");

        var clientePj = service.Cadastrar(
            "Oficina Mecânica Central Ltda",
            "11.222.333/0001-81",
            "(35) 3291-2222",
            "contato@oficinacentral.com.br",
            "Av. Governador Valadares, 500 - Alfenas/MG");

        // Assert
        Assert.NotNull(clientePf);
        Assert.Equal("52998224725", clientePf.CpfCnpj);
        Assert.Equal("11222333000181", clientePj.CpfCnpj);
        Assert.Equal(2, service.ListarTodos().Count);
    }

    [Theory]
    [InlineData("111.111.111-11")]
    [InlineData("123.456.789-00")]
    [InlineData("11.222.333/0001-99")]
    [InlineData("")]
    public void Cadastrar_ComCpfOuCnpjInvalido_DeveLancarArgumentException(string documentoInvalido)
    {
        // Arrange
        var service = new ClienteService();

        // Act & Assert
        var excecao = Assert.Throws<ArgumentException>(() =>
            service.Cadastrar(
                "João Silva",
                documentoInvalido,
                "(35) 98888-0000",
                "joao@email.com",
                "Rua A, 10"));

        Assert.Contains("CPF ou CNPJ", excecao.Message);
    }

    [Theory]
    [InlineData("emailsemarroba.com")]
    [InlineData("cliente@")]
    [InlineData("")]
    public void Cadastrar_ComEmailInvalido_DeveLancarArgumentException(string emailInvalido)
    {
        // Arrange
        var service = new ClienteService();

        // Act & Assert
        var excecao = Assert.Throws<ArgumentException>(() =>
            service.Cadastrar(
                "Maria Souza",
                "52998224725",
                "(35) 98888-0000",
                emailInvalido,
                "Rua B, 20"));

        Assert.Contains("e-mail", excecao.Message);
    }

    [Fact]
    public void Cadastrar_ComCampoObrigatorioVazio_DeveLancarArgumentException()
    {
        // Arrange
        var service = new ClienteService();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.Cadastrar(
                "",
                "52998224725",
                "(35) 98888-0000",
                "maria@email.com",
                "Rua B, 20"));
    }

    [Fact]
    public void Cadastrar_ComDocumentoDuplicado_DeveLancarInvalidOperationException()
    {
        // Arrange
        var service = new ClienteService();
        service.Cadastrar("Carlos Almeida", "529.982.247-25", "(35) 99999-1111", "carlos@email.com", "Rua A, 10");

        // Act & Assert
        var excecao = Assert.Throws<InvalidOperationException>(() =>
            service.Cadastrar("Outro Nome", "52998224725", "(35) 97777-3333", "outro@email.com", "Rua C, 30"));

        Assert.Contains("Já existe um cliente cadastrado", excecao.Message);
    }
    [Theory]
    [InlineData("12345")]
    [InlineData("(35) 999")]
    [InlineData("")]
    public void Cadastrar_ComTelefoneInvalido_DeveLancarArgumentException(string telefoneInvalido)
    {
        var service = new ClienteService();

        var excecao = Assert.Throws<ArgumentException>(() =>
            service.Cadastrar(
                "Lucas Mendes",
                "52998224725",
                telefoneInvalido,
                "lucas@email.com",
                "Rua C, 50"));

        Assert.Contains("telefone", excecao.Message, StringComparison.OrdinalIgnoreCase);
    }    
}