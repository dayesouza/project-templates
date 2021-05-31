using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using TemplateIHD.Domain;

namespace TemplateIHD.Data.Mappings
{
    [ExcludeFromCodeCoverage]
    public class IHDTemplateMap : IEntityTypeConfiguration<IHDEntity>
    {
        public void Configure(EntityTypeBuilder<IHDEntity> builder)
        {
            if (builder is null) return;

            builder.ToTable("JobpackDocuments");

            builder.HasKey(ihd => new { ihd.Id });
            builder.Property(ihd => ihd.Id);

            builder.Property(ihd => ihd.Password)
                .IsRequired()
                .HasColumnName("Password");
        }
    }
}
