using Microsoft.EntityFrameworkCore;
using Mottu.Api.Domain.Entity;
using Mottu.Api.Configurations;

namespace Mottu.Api.Infrastructure.Context;

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