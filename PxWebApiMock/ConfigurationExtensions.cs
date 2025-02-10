using Px.SavedQuery.Backend;
using Px.SavedQuery.Backend.DatabaseAccessors;

namespace PxWebApiMock
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


        public delegate void OptionsDelegate(BackendOptions options);

        public static IServiceCollection AddBackend(this IServiceCollection services, OptionsDelegate? optionsDelegate = null)
        {
            var options = new BackendOptions();
            optionsDelegate?.Invoke(options);

            if (options.Backend.Equals("Database", StringComparison.OrdinalIgnoreCase))
            {
                services.AddTransient<ISavedQueryBackend, DatabaseBackend>();
                if (options.Database is null)
                {
                    throw new InvalidOperationException("Database options must be provided when using Database backend");
                }
                var vendor = options.Database.Vendor;
                string connectionString = options.Database.ConnectionString;
                if (vendor.Equals("Microsoft", StringComparison.OrdinalIgnoreCase))
                {
                    services.AddTransient<ISavedQueryDatabaseAccessor>(provider => new MicrosoftSqlServerDatabaseAccessor(connectionString, options.Database.DataSourceType, options.Database.DatabaseId));
                }
                else if (vendor.Equals("Oracle", StringComparison.OrdinalIgnoreCase))
                {
                    string tableOwner = options.Database.Oracle?.TableOwner ?? "";
                    services.AddTransient<ISavedQueryDatabaseAccessor>(provider => new OracleDatabaseAccessor(connectionString, options.Database.DataSourceType, options.Database.DatabaseId, tableOwner));
                }
            }
            else if (options.Backend.Equals("File", StringComparison.OrdinalIgnoreCase))
            {
                var savedQueryPath = options.File?.Path ?? "";
                services.AddTransient<ISavedQueryBackend>(provider => new FileBackend(savedQueryPath));
            }

            return services;
        }

        public class BackendOptions
        {

            public BackendOptions()
            {
                Backend = "File";
                File = new FileBackendOptions()
                {
                    Path = Path.GetTempPath()
                };
            }

            public string Backend { get; set; }
            public FileBackendOptions? File { get; set; }
            public DatabaseBackendOptions? Database { get; set; }
        }

        public class FileBackendOptions
        {
            public string Path { get; set; }
        }

        public class DatabaseBackendOptions
        {
            public DatabaseBackendOptions(string vendor, string connectionString)
            {
                Vendor = vendor;
                ConnectionString = connectionString;
                DataSourceType = "CNMM";
                DatabaseId = "";
            }
            public string Vendor { get; set; }
            public string ConnectionString { get; set; }

            public string DataSourceType { get; set; }

            public string DatabaseId { get; set; }
            public OracleDatabaseOptions? Oracle { get; set; }
        }

        public class OracleDatabaseOptions
        {
            public string TableOwner { get; set; }
        }
    }
}
