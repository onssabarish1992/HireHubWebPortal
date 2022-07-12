using Microsoft.Extensions.Configuration;
using System.Configuration;


namespace HRAnalytics.DAL
{
    public class DatabaseFactory
    {
    
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        private readonly string providerName;

        public DatabaseFactory(string connectionStringName, IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString(connectionStringName);
            providerName = _configuration.GetConnectionString("ProviderName");
        }

        /// <summary>
        /// Create database depending on database provider
        /// </summary>
        /// <returns></returns>
        public IDatabase CreateDatabase()
        {
            IDatabase database = null;

            switch (providerName.ToLower())
            {
                case "system.data.sqlclient":
                default:
                    database = new SqlServerDatabase(connectionString);
                    break;
            }

            return database;
        }

        /// <summary>
        /// Return the SQL provider name
        /// </summary>
        /// <returns></returns>
        public string GetProviderName()
        {
            return providerName;
        }
    }
}