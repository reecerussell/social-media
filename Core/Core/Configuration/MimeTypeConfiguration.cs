using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configuration
{
    internal class MimeTypeConfiguration : IEntityTypeConfiguration<MimeType>
    {
        public void Configure(EntityTypeBuilder<MimeType> builder)
        {
            builder.ToTable("mime_types");

            builder
                .Property(x => x.Id)
                .HasColumnName("id");

            builder
                .Property(x => x.Name)
                .HasColumnName("name");

            builder
                .Property(x => x.NormalizedName)
                .HasColumnName("normalized_name");
        }
    }
}
