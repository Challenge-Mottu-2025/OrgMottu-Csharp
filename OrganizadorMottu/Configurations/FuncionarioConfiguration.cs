using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizadorMottu.Domain.Entity;

namespace OrganizadorMottu.Configurations;

public class FuncionarioConfiguration : IEntityTypeConfiguration<Funcionario>
{
    public void Configure(EntityTypeBuilder<Funcionario> builder)
    {
        builder.ToTable("T_MT_Funcionario");
        builder.HasKey(f => f.IdFuncionario);

        builder.Property(f => f.IdFuncionario).HasColumnName("ID_FUNCIONARIO").HasPrecision(10);
        builder.Property(f => f.NrCep).HasColumnName("NR_CEP").HasPrecision(8);
        builder.Property(f => f.Senha).HasColumnName("CD_SENHA").HasMaxLength(50);
        builder.Property(f => f.Cpf).HasColumnName("CD_CPF").HasMaxLength(11);
        builder.Property(f => f.Nome).HasColumnName("ID_NOME").HasMaxLength(50);

        builder.HasOne<Endereco>()
               .WithMany()
               .HasForeignKey(f => f.NrCep)
               .HasPrincipalKey(e => e.NrCep);
    }
}