using System.Configuration;

namespace HRAnalytics.DAL
{
    public class DatabaseFactory
    {
        private ConnectionStringSettings connectionStringSettings;

        public DatabaseFactory(string connectionStringName)
        {
            connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
        }

        public IDatabase CreateDatabase()
        {
            IDatabase database = null;

            switch (connectionStringSettings.ProviderName.ToLower())
            {
                case "system.data.sqlclient":
                default:
                    database = new SqlServerDatabase(connectionStringSettings.ConnectionString);
                    break;
            }

            return database;
        }

        public string GetProviderName()
        {
            return connectionStringSettings.ProviderName;
        }
    }
}