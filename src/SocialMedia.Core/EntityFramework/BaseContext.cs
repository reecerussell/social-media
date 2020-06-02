using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Core.Extensions;
using System;
using System.Globalization;

namespace SocialMedia.Core.EntityFramework
{
    public abstract class BaseContext : DbContext
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        protected readonly ILogger<BaseContext> Logger;

        protected BaseContext(
            IConnectionStringProvider connectionStringProvider,
            ILogger<BaseContext> logger)
        {
            _connectionStringProvider = connectionStringProvider;
            Logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?.ToUpper(CultureInfo.InvariantCulture);

            Logger.LogDebug("Configuring database context for environment: " + environment);

            var connectionString = _connectionStringProvider.GetConnectionStringAsync().Result;
            optionsBuilder.UseMySQL(connectionString);
            optionsBuilder.EnableDetailedErrors();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Logger.LogDebug("Configuring tinyint(1) conversion...");
            modelBuilder.ConfigureTinyInt();

            base.OnModelCreating(modelBuilder);
        }
    }
}
