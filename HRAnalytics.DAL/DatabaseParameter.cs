using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.DAL
{
    public class DatabaseParameter
    {
        /// <summary>
        /// Overload - 1 to create data parameter for SQL
        /// </summary>
        /// <param name="argProviderName"></param>
        /// <param name="argName"></param>
        /// <param name="argValue"></param>
        /// <param name="argDbType"></param>
        /// <param name="argDirection"></param>
        /// <returns></returns>
        public static IDbDataParameter CreateParameter(string argProviderName, string argName, object argValue, DbType argDbType, ParameterDirection argDirection = ParameterDirection.Input)
        {
            switch (argProviderName.ToLower())
            {
                case "system.data.sqlclient":
                default:
                    return CreateSqlParameter(argName, argValue, argDbType, argDirection);

            }
        }

        /// <summary>
        /// Overload - 2 to create data parameter for SQL
        /// </summary>
        /// <param name="argProviderName"></param>
        /// <param name="argName"></param>
        /// <param name="argSize"></param>
        /// <param name="argValue"></param>
        /// <param name="argDbType"></param>
        /// <param name="argDirection"></param>
        /// <returns></returns>
        public static IDbDataParameter CreateParameter(string argProviderName, string argName, int argSize, object argValue, DbType argDbType, ParameterDirection argDirection = ParameterDirection.Input)
        {
            switch (argProviderName.ToLower())
            {
                case "system.data.sqlclient":
                default:
                    return CreateSqlParameter(argName, argSize, argValue, argDbType, argDirection);

            }
        }

        /// <summary>
        /// Overload - 1 Create Stored Procedure Parameter
        /// </summary>
        /// <param name="argName"></param>
        /// <param name="argValue"></param>
        /// <param name="argDbType"></param>
        /// <param name="argDirection"></param>
        /// <returns></returns>
        private static IDbDataParameter CreateSqlParameter(string argName, object argValue, DbType argDbType, ParameterDirection argDirection)
        {
            return new SqlParameter
            {
                DbType = argDbType,
                ParameterName = argName,
                Direction = argDirection,
                Value = argValue
            };
        }

        /// <summary>
        /// Overload - 2 Create Stored Procedure Parameter
        /// </summary>
        /// <param name="argName"></param>
        /// <param name="argSize"></param>
        /// <param name="argValue"></param>
        /// <param name="argDbType"></param>
        /// <param name="argDirection"></param>
        /// <returns></returns>
        private static IDbDataParameter CreateSqlParameter(string argName, int argSize, object argValue, DbType argDbType, ParameterDirection argDirection)
        {
            return new SqlParameter
            {
                DbType = argDbType,
                Size = argSize,
                ParameterName = argName,
                Direction = argDirection,
                Value = argValue
            };
        }
    }
}
