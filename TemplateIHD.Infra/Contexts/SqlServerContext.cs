using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TemplateIHD.Domain;
using System.Linq;
using TemplateIHD.Data.Mappings;
using Microsoft.Extensions.Configuration;

namespace TemplateIHD.Data.Contexts
{
    [ExcludeFromCodeCoverage]
    public class SqlServerContext : DbContext
    {
        private readonly IHostEnvironment _env;

        public DbSet<IHDEntity> Jobpacks { get; set; }

        public SqlServerContext() { }

        public SqlServerContext(IHostEnvironment env) => _env = env ??
            throw new ArgumentException("Empty Database Context environment", nameof(env));

        public SqlServerContext(DbContextOptions<SqlServerContext> options) : base(options) { }

        public SqlServerContext(IHostEnvironment env, DbContextOptions<SqlServerContext> options) : base(options)
        {
            _env = env;
            if (env is null)
                throw new ArgumentException("Empty IHostEnvironment");

            if (env.EnvironmentName != "Local")
            {
                var connection = (SqlConnection)Database.GetDbConnection();
                connection.AccessToken = (new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
                throw new ArgumentException("Empty Database model builder", nameof(modelBuilder));

            modelBuilder.ApplyConfiguration(new IHDTemplateMap());

            //Disable cascade deletion
            var cascadeFKs = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            base.OnModelCreating(modelBuilder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_env is null) return;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Build();
        }
    }
}
