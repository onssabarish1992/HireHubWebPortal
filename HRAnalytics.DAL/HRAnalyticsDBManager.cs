using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.DAL
{
    public class HRAnalyticsDBManager
    {
        private DatabaseFactory _dbFactory;
        private IDatabase _database;
        private string _providerName;
        private readonly IConfiguration _configuration;

        public HRAnalyticsDBManager(string argConnectionStringName, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbFactory = new DatabaseFactory(argConnectionStringName, _configuration);
            _database = _dbFactory.CreateDatabase();
            _providerName = _dbFactory.GetProviderName();
        }

        /// <summary>
        /// Get databse connections from connection pool
        /// </summary>
        /// <returns></returns>
        public IDbConnection GetDatabasecOnnection()
        {
            return _database.CreateConnection();
        }

        /// <summary>
        /// Close the database connection post database operations
        /// </summary>
        /// <param name="argConnection"></param>
        public void CloseConnection(IDbConnection argConnection)
        {
            _database.CloseConnection(argConnection);
        }

        /// <summary>
        /// Create Stored procedure parameter - Overload 1
        /// </summary>
        /// <param name="argName"></param>
        /// <param name="argValue"></param>
        /// <param name="argDbType"></param>
        /// <returns></returns>
        public IDbDataParameter CreateParameter(string argName, object argValue, DbType argDbType)
        {
            return DatabaseParameter.CreateParameter(_providerName, argName, argValue, argDbType, ParameterDirection.Input);
        }

        /// <summary>
        /// Create Stored procedure parameter - Overload 2
        /// </summary>
        /// <param name="argName"></param>
        /// <param name="argSize"></param>
        /// <param name="argValue"></param>
        /// <param name="argDbType"></param>
        /// <returns></returns>
        public IDbDataParameter CreateParameter(string argName, int argSize, object argValue, DbType argDbType)
        {
            return DatabaseParameter.CreateParameter(_providerName, argName, argSize, argValue, argDbType, ParameterDirection.Input);
        }

        /// <summary>
        /// Create Stored procedure parameter - Overload 3
        /// </summary>
        /// <param name="argName"></param>
        /// <param name="argSize"></param>
        /// <param name="argValue"></param>
        /// <param name="argDbType"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public IDbDataParameter CreateParameter(string argName, int argSize, object argValue, DbType argDbType, ParameterDirection direction)
        {
            return DatabaseParameter.CreateParameter(_providerName, argName, argSize, argValue, argDbType, direction);
        }

        /// <summary>
        /// Generic operation to get data into datatable using stored procedure
        /// </summary>
        /// <param name="argCommandText"></param>
        /// <param name="argCommandType"></param>
        /// <param name="argParameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string argCommandText, CommandType argCommandType, IDbDataParameter[] argParameters = null)
        {
            using (var connection = _database.CreateConnection())
            {
                connection.Open();

                using (var command = _database.CreateCommand(argCommandText, argCommandType, connection))
                {
                    if (argParameters != null)
                    {
                        foreach (var parameter in argParameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    var dataset = new DataSet();
                    var dataAdaper = _database.CreateAdapter(command);
                    dataAdaper.Fill(dataset);

                    return dataset.Tables[0];
                }
            }
        }

        /// <summary>
        /// Generic operation to get data into dataset using stored procedure
        /// </summary>
        /// <param name="argCommandText"></param>
        /// <param name="argCommandType"></param>
        /// <param name="argParameters"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string argCommandText, CommandType argCommandType, IDbDataParameter[] argParameters = null)
        {
            using (var connection = _database.CreateConnection())
            {
                connection.Open();

                using (var command = _database.CreateCommand(argCommandText, argCommandType, connection))
                {
                    if (argParameters != null)
                    {
                        foreach (var parameter in argParameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    var dataset = new DataSet();
                    var dataAdaper = _database.CreateAdapter(command);
                    dataAdaper.Fill(dataset);

                    return dataset;
                }
            }
        }

        /// <summary>
        /// Generic operation to delete SQL Stored procedure
        /// </summary>
        /// <param name="argCommandText"></param>
        /// <param name="argCommandType"></param>
        /// <param name="argParameters"></param>
        public void Delete(string argCommandText, CommandType argCommandType, IDbDataParameter[] argParameters = null)
        {
            using (var connection = _database.CreateConnection())
            {
                connection.Open();

                using (var command = _database.CreateCommand(argCommandText, argCommandType, connection))
                {
                    if (argParameters != null)
                    {
                        foreach (var parameter in argParameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Generic operation to insert SQL Stored procedure
        /// </summary>
        /// <param name="argCommandText"></param>
        /// <param name="argCommandType"></param>
        /// <param name="argParameters"></param>
        public void Insert(string argCommandText, CommandType argCommandType, IDbDataParameter[] argParameters)
        {
            using (var connection = _database.CreateConnection())
            {
                connection.Open();

                using (var command = _database.CreateCommand(argCommandText, argCommandType, connection))
                {
                    if (argParameters != null)
                    {
                        foreach (var parameter in argParameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Generic operation to delete SQL Stored procedure - Overload 1
        /// </summary>
        /// <param name="argCommandText"></param>
        /// <param name="argCommandType"></param>
        /// <param name="argParameters"></param>
        /// <param name="argLastId"></param>
        /// <returns></returns>
        public int Insert(string argCommandText, CommandType argCommandType, IDbDataParameter[] argParameters, out int argLastId)
        {
            argLastId = 0;
            using (var connection = _database.CreateConnection())
            {
                connection.Open();

                using (var command = _database.CreateCommand(argCommandText, argCommandType, connection))
                {
                    if (argParameters != null)
                    {
                        foreach (var parameter in argParameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    object newId = command.ExecuteScalar();
                    argLastId = Convert.ToInt32(newId);
                }
            }

            return argLastId;
        }

        /// <summary>
        /// Generic operation to update SQL Stored procedure
        /// </summary>
        /// <param name="argCommandText"></param>
        /// <param name="argCommandType"></param>
        /// <param name="argParameters"></param>
        public void Update(string argCommandText, CommandType argCommandType, IDbDataParameter[] argParameters)
        {
            using (var connection = _database.CreateConnection())
            {
                connection.Open();

                using (var command = _database.CreateCommand(argCommandText, argCommandType, connection))
                {
                    if (argParameters != null)
                    {
                        foreach (var parameter in argParameters)
                        {
                            command.Parameters.Add(parameter);
                        }
                    }

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
