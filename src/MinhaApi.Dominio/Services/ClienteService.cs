using MinhaApi.Dominio.Entities;
using MinhaApi.Dominio.Validators;

namespace MinhaApi.Dominio.Services;

public sealed class ClienteService
{
    private readonly List<Cliente> _clientes = new();
    private readonly object _lock = new();

    public Cliente Cadastrar(string nome, string cpfCnpj, string telefone, string email, string endereco)
    {
        lock (_lock)
        {
            var documentoLimpo = DocumentoValidator.LimparFormatacao(cpfCnpj);

            if (_clientes.Any(c => c.CpfCnpj == documentoLimpo))
                throw new InvalidOperationException("Já existe um cliente cadastrado com este CPF ou CNPJ.");

            var cliente = new Cliente(nome, cpfCnpj, telefone, email, endereco);
            _clientes.Add(cliente);
            return cliente;
        }
    }

    public Cliente? BuscarPorId(Guid id)
    {
        lock (_lock)
        {
            return _clientes.FirstOrDefault(c => c.Id == id);
        }
    }

    public Cliente? BuscarPorDocumento(string cpfCnpj)
    {
        lock (_lock)
        {
            var documentoLimpo = DocumentoValidator.LimparFormatacao(cpfCnpj);
            return _clientes.FirstOrDefault(c => c.CpfCnpj == documentoLimpo);
        }
    }

    public IReadOnlyCollection<Cliente> ListarTodos()
    {
        lock (_lock)
        {
            return _clientes.ToArray();
        }
    }
}