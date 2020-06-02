using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Configuration
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder
                .Property(x => x.Id)
                .HasColumnName("id");

            builder
                .Property(x => x.Username)
                .HasColumnName("username");

            builder
                .Property(x => x.NormalizedUsername)
                .HasColumnName("normalized_username");

            builder
                .Property(x => x.PasswordHash)
                .HasColumnName("password_hash");

            builder
                .Property(x => x.Bio)
                .HasColumnName("bio");

            builder
                .Property(x => x.LockedOut)
                .HasColumnName("locked_out");

            builder
                .Property(x => x.LockoutEnd)
                .HasColumnName("lockout_end");

            builder
                .Property(x => x.AccessFailedCount)
                .HasColumnName("access_failed_count");

            builder
                .Property(x => x.ProfileImageId)
                .HasColumnName("profile_image_id");

            builder
                .Property(x => x.ConcurrencyStamp)
                .HasColumnName("concurrency_stamp")
                .IsConcurrencyToken();

            builder
                .Property(x => x.Snapchat)
                .HasColumnName("snapchat");

            builder
                .Property(x => x.Instagram)
                .HasColumnName("instagram");

            builder
                .Property(x => x.Twitter)
                .HasColumnName("twitter");

            builder
                .HasMany(x => x.Followers)
                .WithOne()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(x => x.LoginAttempts)
                .WithOne()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(x => x.Posts)
                .WithOne()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.ProfileImage)
                .WithMany()
                .HasForeignKey(x => x.ProfileImageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
