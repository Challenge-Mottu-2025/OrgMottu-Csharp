using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganizadorMottu.Domain.Entity;

namespace OrganizadorMottu.Configurations;

public class MotoConfiguration : IEntityTypeConfiguration<Moto>
{
    public void Configure(EntityTypeBuilder<Moto> builder)
    {
        builder.ToTable("T_MT_MOTO");
        builder.HasKey(m => m.Placa);

        builder.Property(m => m.Placa).HasColumnName("CD_PLACA").HasMaxLength(7).IsRequired();
        builder.Property(m => m.Cpf).HasColumnName("CD_CPF").HasMaxLength(11);
        builder.Property(m => m.Nv).HasColumnName("CD_NV").HasMaxLength(7);
        builder.Property(m => m.Motor).HasColumnName("CD_MOTOR").HasMaxLength(12);
        builder.Property(m => m.Renavam).HasColumnName("CD_RENAVAM").HasPrecision(11);
        builder.Property(m => m.Fipe).HasColumnName("CD_FIPE").HasPrecision(7);

        // Moto (CD_CPF) -> Usuario (CD_CPF) (N:1 opcional)
        builder.HasOne(m => m.Usuario)
               .WithMany()
               .HasForeignKey(m => m.Cpf)
               .HasPrincipalKey(u => u.Cpf);
    }
}