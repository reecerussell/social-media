using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq;

namespace Core.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ConfigureTinyInt(this ModelBuilder builder)
        {
            var entityTypes = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in entityTypes)
            {
                var properties = entityType.GetProperties().ToList();
                var entityTypeBuilder = new EntityTypeBuilder(entityType);

                foreach (var property in properties)
                {
                    if (property.PropertyInfo == null)
                    {
                        continue;
                    }

                    if (!property.PropertyInfo.PropertyType.IsBoolean())
                    {
                        continue;
                    }

                    entityTypeBuilder
                        .Property(property.Name)
                        .HasConversion(new BoolToZeroOneConverter<short>())
                        .HasColumnType("tinyint(1)");
                }
            }
        }
    }
}
