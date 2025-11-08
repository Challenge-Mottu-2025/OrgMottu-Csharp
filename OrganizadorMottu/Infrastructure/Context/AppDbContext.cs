using Microsoft.EntityFrameworkCore;
using OrganizadorMottu.Configurations;
using OrganizadorMottu.Domain.Entity;

namespace OrganizadorMottu.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Moto> Motos => Set<Moto>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();
    public DbSet<Funcionario> Funcionarios => Set<Funcionario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
        modelBuilder.ApplyConfiguration(new MotoConfiguration());
        modelBuilder.ApplyConfiguration(new EnderecoConfiguration());
        modelBuilder.ApplyConfiguration(new FuncionarioConfiguration());
    }
}