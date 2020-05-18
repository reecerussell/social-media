using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configuration
{
    internal class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
    {
        public void Configure(EntityTypeBuilder<PostLike> builder)
        {
            builder.ToTable("post_likes");

            builder.HasKey(x => new {x.UserId, x.PostId});

            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id");

            builder
                .Property(x => x.PostId)
                .HasColumnName("post_id");

            builder
                .Property(x => x.Date)
                .HasColumnName("date");
        }
    }
}
