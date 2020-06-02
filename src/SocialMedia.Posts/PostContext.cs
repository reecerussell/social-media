using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SocialMedia.Core;
using SocialMedia.Core.EntityFramework;
using SocialMedia.Domain.Configuration;

namespace SocialMedia.Posts
{
    internal class PostContext : BaseContext
    {
        public PostContext(
            IConnectionStringProvider connectionStringProvider,
            ILogger<PostContext> logger)
            : base(
                connectionStringProvider,
                logger)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Logger.LogDebug("Configuring entity models...");
            modelBuilder.ConfigureDomain();

            base.OnModelCreating(modelBuilder);
        }
    }
}
