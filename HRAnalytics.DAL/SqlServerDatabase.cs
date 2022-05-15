using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.DAL
{
    public class SqlServerDatabase: IDatabase
    {
        private string ConnectionString { get; set; }

        public void CloseConnection(IDbConnection argConnection)
        {
            var sqlConnection = (SqlConnection)argConnection;
            sqlConnection.Close();
            sqlConnection.Dispose();
        }

        public IDataAdapter CreateAdapter(IDbCommand argCommand)
        {
            return new SqlDataAdapter((SqlCommand)argCommand);
        }

        public IDbCommand CreateCommand(string argCommandText, CommandType argCommandType, IDbConnection argConnection)
        {
            return new SqlCommand
            {
                CommandText = argCommandText,
                Connection = (SqlConnection)argConnection,
                CommandType = argCommandType
            };
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public SqlServerDatabase(string argConnectionString)
        {
            ConnectionString = argConnectionString;
        }

        public IDbDataParameter CreateParameter(IDbCommand argcommand)
        {
            SqlCommand SQLcommand = (SqlCommand)argcommand;
            return SQLcommand.CreateParameter();
        }
    }
}
