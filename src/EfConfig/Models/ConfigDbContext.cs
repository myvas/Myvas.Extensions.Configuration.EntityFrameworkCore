using Microsoft.EntityFrameworkCore;

namespace Myvas.Extensions.Configuration.EntityFrameworkCore
{
    public class ConfigDbContext : DbContext
    {
        public ConfigDbContext(DbContextOptions<ConfigDbContext> options) : base(options) { }

        public DbSet<ConfigurationValue> ConfigurationValues { get; set; }

        /// <param name="builder"></param>
        /// <remarks>当此函数的实现发生改变时，需要执行以下命令才会被执行：
        /// <code>
        /// dotnet ef migrations add Xxx
        /// dotnet ef database update
        /// </code>
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<ConfigurationValue>(b =>
            {
                b.HasKey(u => u.Key);
                b.HasIndex(u => u.Key).HasName("KeyIndex").IsUnique();
                b.ToTable("ConfigurationValues");
                b.Property(u => u.Key).HasMaxLength(256);
            });
        }
    }
}
