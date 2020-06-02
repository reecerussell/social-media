using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Configuration
{
    internal class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.ToTable("media");

            builder
                .Property(x => x.Id)
                .HasColumnName("id");

            builder
                .Property(x => x.Description)
                .HasColumnName("description");

            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id");

            builder
                .Property(x => x.MimeTypeId)
                .HasColumnName("mime_type_id");

            builder
                .HasOne(x => x.MimeType)
                .WithMany()
                .HasForeignKey(x => x.MimeTypeId);
        }
    }
}
