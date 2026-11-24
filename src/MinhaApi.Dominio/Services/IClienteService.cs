using MinhaApi.Dominio.Entities;

namespace MinhaApi.Dominio.Services;

public interface IClienteService
{
    Task<Cliente> CadastrarAsync(
        string nome,
        string cpfCnpj,
        string telefone,
        string email,
        string endereco,
        CancellationToken cancellationToken);

    Task<Cliente?> BuscarPorIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Cliente?> BuscarPorDocumentoAsync(string cpfCnpj, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Cliente>> ListarTodosAsync(CancellationToken cancellationToken);
}
