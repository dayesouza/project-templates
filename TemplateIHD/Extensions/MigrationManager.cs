using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TemplateIHD.Data.Contexts;
using TemplateIHD.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace TemplateIHD.WebAPI.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            if (host is null) throw new Exception("Failed during database migration. Host variable was not properly set");

            try
            {
                using var scope = host.Services.CreateScope();

                using var appContext = scope.ServiceProvider.GetRequiredService<SqlServerContext>();

                appContext.Database.Migrate();

                return host;
            }
            catch (Exception e)
            {
                throw new Exception("Failed during database migration", e);
            }
        }
    }
}
