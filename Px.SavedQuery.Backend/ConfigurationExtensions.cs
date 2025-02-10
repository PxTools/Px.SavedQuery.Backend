using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Px.SavedQuery.Backend.DatabaseAccessors;

namespace Px.SavedQuery.Backend
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Example configuration:
        ///  "SavedQuery": {
        ///    "Backend": "File",
        ///    "File": {
        ///      "Path": ""
        ///    },
        ///    "Database": {
        ///      "Vendor": "Oracle",
        ///      "ConnectionString": "MyConnectionString"
        ///    }
        ///  }
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddBackend(this IServiceCollection services, IConfiguration configuration)
        {
            var backendType = configuration["SavedQuery:Backend"];
            if (backendType == "Database")
            {
                services.AddTransient<ISavedQueryBackend, DatabaseBackend>();
                string vendor = configuration["SavedQuery:Database:Vendor"] ?? "";
                string connectionString = configuration["SavedQuery:Database:ConnectionString"] ?? "";
                if (vendor == "Microsoft")
                {
                    services.AddTransient<ISavedQueryDatabaseAccessor>(provider => new MicrosoftSqlServerDatabaseAccessor(connectionString, "TODO", "TODO"));
                }
                else if (vendor == "Oracle")
                {
                    string tableOwner = configuration["SavedQuery:Database:Oracle:TableOwner"] ?? "";
                    services.AddTransient<ISavedQueryDatabaseAccessor>(provider => new OracleDatabaseAccessor(connectionString, "TODO", "TODO", tableOwner));
                }
            }
            else if (backendType == "File")
            {
                var savedQueryPath = configuration["SavedQuery:File:Path"] ?? "";
                services.AddTransient<ISavedQueryBackend>(provider => new FileBackend(savedQueryPath));
            }

            return services;
        }
    }
}
