namespace MinhaApi.Dominio.DTOs.Clientes;

public sealed record ClienteResponse(
    Guid Id,
    string Nome,
    string CpfCnpj,
    string Telefone,
    string Email,
    string Endereco);
