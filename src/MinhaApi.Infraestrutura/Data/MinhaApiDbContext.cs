using Microsoft.EntityFrameworkCore;
using MinhaApi.Dominio.Entities;

namespace MinhaApi.Infraestrutura.Data;

public sealed class MinhaApiDbContext(DbContextOptions<MinhaApiDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(cliente => cliente.Id);
            entity.Property(cliente => cliente.Id).ValueGeneratedNever();
            entity.Property(cliente => cliente.Nome).IsRequired();
            entity.Property(cliente => cliente.CpfCnpj).IsRequired();
            entity.Property(cliente => cliente.Telefone).IsRequired();
            entity.Property(cliente => cliente.Email).IsRequired();
            entity.Property(cliente => cliente.Endereco).IsRequired();
            entity.HasIndex(cliente => cliente.CpfCnpj).IsUnique();
        });

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.ToTable("Produtos");
            entity.HasKey(produto => produto.Id);
            entity.Property(produto => produto.Id).ValueGeneratedNever();
            entity.Property(produto => produto.Nome).IsRequired();
            entity.Property(produto => produto.CodigoBarras)
                .IsRequired()
                .UseCollation("NOCASE");
            entity.Property(produto => produto.NumeroSerie).IsRequired();
            entity.Property(produto => produto.Preco).HasPrecision(18, 2);
            entity.HasIndex(produto => produto.CodigoBarras).IsUnique();
        });

        modelBuilder.Entity<Venda>(entity =>
        {
            entity.ToTable("Vendas");
            entity.HasKey(venda => venda.Id);
            entity.Property(venda => venda.Id).ValueGeneratedNever();
            entity.Property(venda => venda.DataVenda).IsRequired();
            entity.Property(venda => venda.Desconto).HasPrecision(18, 2);
            entity.HasOne(venda => venda.Cliente)
                .WithMany()
                .HasForeignKey("ClienteId")
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(venda => venda.Itens)
                .WithOne()
                .HasForeignKey("VendaId")
                .OnDelete(DeleteBehavior.Cascade);
            entity.Navigation(venda => venda.Itens)
                .HasField("_itens")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<ItemVenda>(entity =>
        {
            entity.ToTable("ItensVenda");
            entity.HasKey("VendaId", "ProdutoId");
            entity.Property(item => item.PrecoUnitario).HasPrecision(18, 2);
            entity.HasOne(item => item.Produto)
                .WithMany()
                .HasForeignKey("ProdutoId")
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
