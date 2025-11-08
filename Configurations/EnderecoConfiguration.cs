using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mottu.Api.Domain.Entity;

namespace Mottu.Api.Configurations;

public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.ToTable("T_MT_Endereco");
        builder.HasKey(e => e.NrCep);

        builder.Property(e => e.NrCep).HasColumnName("NR_CEP").HasPrecision(8).IsRequired();
        builder.Property(e => e.IdPais).HasColumnName("ID_PAIS").HasMaxLength(35);
        builder.Property(e => e.SiglaEstado).HasColumnName("SG_ESTADO").HasMaxLength(2).IsFixedLength();
        builder.Property(e => e.IdCidade).HasColumnName("ID_CIDADE").HasMaxLength(50);
        builder.Property(e => e.IdBairro).HasColumnName("ID_BAIRRO").HasMaxLength(50);
        builder.Property(e => e.NrNumero).HasColumnName("NR_NUMERO").HasMaxLength(10);
        builder.Property(e => e.Logradouro).HasColumnName("DS_LOGRADOURO").HasMaxLength(50);
        builder.Property(e => e.Complemento).HasColumnName("DS_COMPLEMENTO").HasMaxLength(25);
    }
}