using Microsoft.EntityFrameworkCore;
using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Services;
using MinhaApi.Dominio.Validators;
using MinhaApi.Infraestrutura.Data;

namespace MinhaApi.Infraestrutura.Services;

public sealed class ClientePersistenceService(MinhaApiDbContext dbContext) : IClienteService
{
    public async Task<Cliente> CadastrarAsync(
        string nome,
        string cpfCnpj,
        string telefone,
        string email,
        string endereco,
        CancellationToken cancellationToken)
    {
        var documentoLimpo = DocumentoValidator.LimparFormatacao(cpfCnpj);
        if (await dbContext.Clientes.AnyAsync(cliente => cliente.CpfCnpj == documentoLimpo, cancellationToken))
            throw new InvalidOperationException("Já existe um cliente cadastrado com este CPF ou CNPJ.");

        var cliente = new Cliente(nome, cpfCnpj, telefone, email, endereco);
        dbContext.Clientes.Add(cliente);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqliteConstraintErrors.IsUniqueConstraint(exception))
        {
            dbContext.Entry(cliente).State = EntityState.Detached;
            throw new InvalidOperationException("Já existe um cliente cadastrado com este CPF ou CNPJ.", exception);
        }

        return cliente;
    }

    public Task<Cliente?> BuscarPorIdAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Clientes.FirstOrDefaultAsync(cliente => cliente.Id == id, cancellationToken);

    public async Task<Cliente?> BuscarPorDocumentoAsync(string cpfCnpj, CancellationToken cancellationToken)
    {
        var documentoLimpo = DocumentoValidator.LimparFormatacao(cpfCnpj);
        return await dbContext.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(cliente => cliente.CpfCnpj == documentoLimpo, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Cliente>> ListarTodosAsync(CancellationToken cancellationToken) =>
        await dbContext.Clientes.AsNoTracking().ToArrayAsync(cancellationToken);
}
