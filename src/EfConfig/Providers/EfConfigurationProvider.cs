using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Myvas.Extensions.Configuration.EntityFrameworkCore
{
    /// <summary>
    /// 1.Load()
    /// 2.Set(key, value)
    /// 3.Save()
    /// </summary>
    public class EfConfigurationProvider : ConfigurationProvider, IEnumerable<KeyValuePair<string, string>>
    {
        public EfConfigurationSource Source { get; }

        public EfConfigurationProvider(EfConfigurationSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));

            if (Source.InitialData != null)
            {
                foreach (var pair in Source.InitialData)
                {
                    Data.Add(pair.Key, pair.Value);
                }
            }
        }

        public override void Set(string key, string value)
        {
            var builder = new DbContextOptionsBuilder<ConfigDbContext>();
            Source.OptionsAction(builder);

            using (var dbContext = new ConfigDbContext(builder.Options))
            {
                dbContext.ConfigurationValues.Add(new ConfigurationValue() { Key = key, Value = value });
                dbContext.SaveChanges();
            }
            Load();
        }

        public override bool TryGet(string key, out string value)
        {
            Load();
            return Data.TryGetValue(key, out value);
        }

        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<ConfigDbContext>();
            Source.OptionsAction(builder);

            using (var dbContext = new ConfigDbContext(builder.Options))
            {
                //dbContext.Database.EnsureCreated();
                if (dbContext.Database.EnsureCreated())
                {
                }
                Data = dbContext.ConfigurationValues.Any()
                    ? dbContext.ConfigurationValues.ToDictionary(c => c.Key, c => c.Value)
                    : InitData(dbContext);
            }
        }

        private void InsertOrUpdateData(ConfigDbContext dbContext)
        {
            var values = dbContext.ConfigurationValues.ToArray();
            dbContext.ConfigurationValues.RemoveRange(values);

            dbContext.ConfigurationValues.AddRange(Data
                .Select(kvp => new ConfigurationValue { Key = kvp.Key, Value = kvp.Value })
                .ToArray());

            dbContext.SaveChanges();
        }

        private IDictionary<string, string> InitData(ConfigDbContext dbContext)
        {
            Data = Source.InitialData;

            InsertOrUpdateData(dbContext);

            return Data;
        }

        #region implementation of IEnumerable<KeyValuePair<string, string>>
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}