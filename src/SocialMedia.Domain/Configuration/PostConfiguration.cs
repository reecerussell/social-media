using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Configuration
{
    internal class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("posts");

            builder
                .Property(x => x.Id)
                .HasColumnName("id");

            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id");

            builder
                .Property(x => x.MediaId)
                .HasColumnName("media_id");

            builder
                .Property(x => x.DateCreated)
                .HasColumnName("date_created");

            builder
                .Property(x => x.Caption)
                .HasColumnName("caption");

            builder
                .HasMany(x => x.Comments)
                .WithOne()
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(x => x.Likes)
                .WithOne()
                .HasForeignKey(x => x.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.Media)
                .WithOne()
                .HasForeignKey<Post>(x => x.MediaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
