using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SocialMedia.Core;
using SocialMedia.Core.EntityFramework;
using SocialMedia.Domain.Configuration;

namespace SocialMedia.Auth
{
    public class UserContext : BaseContext
    {
        public UserContext(
            IConnectionStringProvider connectionStringProvider,
            ILogger<UserContext> logger) 
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
