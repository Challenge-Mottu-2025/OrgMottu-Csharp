using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mottu.Api.Models;

namespace Mottu.Api.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("T_MT_Usuario");
        builder.HasKey(u => u.Cpf);

        builder.Property(u => u.Cpf).HasColumnName("CD_CPF").HasMaxLength(11).IsRequired();
        builder.Property(u => u.NrCep).HasColumnName("NR_CEP").HasPrecision(8);
        builder.Property(u => u.CdPlaca).HasColumnName("CD_PLACA").HasMaxLength(7);
        builder.Property(u => u.Nome).HasColumnName("ID_NOME").HasMaxLength(50).IsRequired();
        builder.Property(u => u.DataNascimento).HasColumnName("DT_NASCIMENTO").HasColumnType("DATE");

        // Usuario -> Endereco (NR_CEP)
        builder.HasOne(u => u.Endereco)
               .WithMany(e => e.Usuarios)
               .HasForeignKey(u => u.NrCep)
               .HasPrincipalKey(e => e.NrCep);

        // Usuario (CD_PLACA) -> Moto (CD_PLACA) (1:1 opcional)
        builder.HasOne(u => u.Moto)
               .WithOne(m => m.UsuarioByPlaca)
               .HasForeignKey<Usuario>(u => u.CdPlaca)
               .HasPrincipalKey<Moto>(m => m.Placa);

        builder.Navigation(u => u.Endereco).AutoInclude(false);
        builder.Navigation(u => u.Moto).AutoInclude(false);
    }
}