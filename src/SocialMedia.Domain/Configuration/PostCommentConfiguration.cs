using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Configuration
{
    internal class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
    {
        public void Configure(EntityTypeBuilder<PostComment> builder)
        {
            builder.ToTable("post_comments");

            builder
                .Property(x => x.Id)
                .HasColumnName("id");

            builder
                .Property(x => x.PostId)
                .HasColumnName("post_id");

            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id");

            builder
                .Property(x => x.ReplyToId)
                .HasColumnName("reply_to_id");

            builder
                .Property(x => x.DateCreated)
                .HasColumnName("date_created");

            builder
                .Property(x => x.Text)
                .HasColumnName("text");

            builder
                .HasMany(x => x.Replies)
                .WithOne()
                .HasForeignKey(x => x.ReplyToId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
