using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Myvas.Extensions.Configuration.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.Configuration
{
    public static class EfConfigurationBuilderExtensions
    {
        /// <summary>
        /// Adds an Microsoft.Extensions.Configuration.IConfigurationProvider that reads 
        /// configuration values from database via EntityFrameworkCore.
        /// </summary>
        /// <param name="builder">The Microsoft.Extensions.Configuration.IConfigurationBuilder to add to.</param>
        /// <param name="setup"></param>
        /// <returns>The Microsoft.Extensions.Configuration.IConfigurationBuilder.</returns>
        public static IConfigurationBuilder AddEntityFrameworkCore(
            this IConfigurationBuilder builder,
            Action<DbContextOptionsBuilder> setup,
            IDictionary<string, string> initialData = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            
            var source = new EfConfigurationSource(setup) { InitialData = initialData };
            builder.Add(source);
            return builder;
        }
    }
}