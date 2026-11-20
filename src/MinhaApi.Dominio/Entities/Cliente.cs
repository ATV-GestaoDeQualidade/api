using MinhaApi.Dominio.Validators;

namespace MinhaApi.Dominio.Entities;

public class Cliente
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string CpfCnpj { get; private set; }
    public string Telefone { get; private set; }
    public string Email { get; private set; }
    public string Endereco { get; private set; }

    public Cliente(string nome, string cpfCnpj, string telefone, string email, string endereco)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do cliente é obrigatório.", nameof(nome));

        if (!DocumentoValidator.EhCpfOuCnpjValido(cpfCnpj))
            throw new ArgumentException("O CPF ou CNPJ informado é inválido.", nameof(cpfCnpj));

        if (string.IsNullOrWhiteSpace(telefone))
            throw new ArgumentException("O telefone do cliente é obrigatório.", nameof(telefone));

        if (!DocumentoValidator.EhEmailValido(email))
            throw new ArgumentException("O e-mail informado possui formato inválido.", nameof(email));

        if (string.IsNullOrWhiteSpace(endereco))
            throw new ArgumentException("O endereço do cliente é obrigatório.", nameof(endereco));

        Id = Guid.NewGuid();
        Nome = nome.Trim();
        CpfCnpj = DocumentoValidator.LimparFormatacao(cpfCnpj);
        Telefone = telefone.Trim();
        Email = email.Trim().ToLowerInvariant();
        Endereco = endereco.Trim();
    }
}