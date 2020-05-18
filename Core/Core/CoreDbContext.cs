using Core.Configuration;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;

namespace Core
{
    public class CoreDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly ISecretProvider _secretProvider;

        public CoreDbContext(
            IConfiguration configuration,
            ISecretProvider secretProvider)
        {
            _configuration = configuration;
            _secretProvider = secretProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString;
            switch (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToUpper(CultureInfo.InvariantCulture))
            {
                case "DEVELOPMENT":
                    connectionString = _secretProvider.GetMySqlConnectionStringAsync("dev/SocialMedia/mysql").Result;
                    break;
                case "PRODUCTION":
                    connectionString = _secretProvider.GetMySqlConnectionStringAsync("prod/SocialMedia/mysql").Result;
                    break;
                case "TEST":
                    connectionString = _configuration["ConnectionStrings:DefaultConnection"];
                    break;
                default:
                    throw new ArgumentException("No connection string is configured for this environment");
            }

            optionsBuilder.UseMySQL(connectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MimeTypeConfiguration());
            modelBuilder.ApplyConfiguration(new MediaConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserFollowerConfiguration());
            modelBuilder.ApplyConfiguration(new PostCommentConfiguration());
            modelBuilder.ApplyConfiguration(new PostLikeConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());

            modelBuilder.ConfigureTinyInt();
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
