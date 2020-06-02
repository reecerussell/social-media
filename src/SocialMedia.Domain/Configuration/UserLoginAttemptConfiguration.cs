using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Domain.Models;

namespace SocialMedia.Domain.Configuration
{
    internal class UserLoginAttemptConfiguration : IEntityTypeConfiguration<UserLoginAttempt>
    {
        public void Configure(EntityTypeBuilder<UserLoginAttempt> builder)
        {
            builder.ToTable("user_login_attempts");

            builder
                .Property(x => x.Id)
                .HasColumnName("id");

            builder
                .Property(x => x.UserId)
                .HasColumnName("user_id");

            builder
                .Property(x => x.RemoteAddress)
                .HasColumnName("remote_address");

            builder
                .Property(x => x.Successful)
                .HasColumnName("successful");

            builder
                .Property(x => x.Message)
                .HasColumnName("message");

            builder
                .Property(x => x.Date)
                .HasColumnName("date");
        }
    }
}
