using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Configuration
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
                .HasMany(x => x.Followers)
                .WithOne()
                .HasForeignKey(x => x.UserId);
        }
    }
}
