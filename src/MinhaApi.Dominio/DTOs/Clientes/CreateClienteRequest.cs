namespace MinhaApi.Dominio.DTOs.Clientes;

public sealed record CreateClienteRequest
{
    public required string Nome { get; init; }
    public required string CpfCnpj { get; init; }
    public required string Telefone { get; init; }
    public required string Email { get; init; }
    public required string Endereco { get; init; }
}
