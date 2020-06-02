using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Domain.Configuration
{
    public static class Configuration
    {
        public static void ConfigureDomain(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MimeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MediaConfiguration());
            modelBuilder.ApplyConfiguration(new UserLoginAttemptConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserFollowerConfiguration());
            modelBuilder.ApplyConfiguration(new PostCommentConfiguration());
            modelBuilder.ApplyConfiguration(new PostLikeConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
        }
    }
}
