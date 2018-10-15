using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Myvas.Extensions.Configuration.EntityFrameworkCore
{
    public class ConfigDbContextSeed
    {
        public static async Task SeedAsync(ConfigDbContext db,
            ILoggerFactory loggerFactory, int? retry = 0)
        {
            db.Database.EnsureCreated();

            int retryForAvailability = retry.Value;
            try
            {
                // TODO: Only run this if using a real database
                // context.Database.Migrate();

                if (!db.ConfigurationValues.Any())
                {
                    db.ConfigurationValues.AddRange(
                        GetPreconfiguredCatalogBrands());

                    await db.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<ConfigDbContextSeed>();
                    log.LogError(ex.Message);
                    await SeedAsync(db, loggerFactory, retryForAvailability);
                }
            }
        }

        private static IEnumerable<ConfigurationValue> GetPreconfiguredCatalogBrands()
        {
            return new List<ConfigurationValue>()
            {
                new ConfigurationValue() { Key = "Domain", Value="ruhu.gz.cn"},
                new ConfigurationValue() { Key = "Https", Value="true"}
            };
        }
    }
}
