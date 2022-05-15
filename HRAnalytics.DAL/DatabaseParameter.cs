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
        public static IDbDataParameter CreateParameter(string argProviderName, string argName, object argValue, DbType argDbType, ParameterDirection argDirection = ParameterDirection.Input)
        {
            switch (argProviderName.ToLower())
            {
                case "system.data.sqlclient":
                default:
                    return CreateSqlParameter(argName, argValue, argDbType, argDirection);

            }
        }

        public static IDbDataParameter CreateParameter(string argProviderName, string argName, int argSize, object argValue, DbType argDbType, ParameterDirection argDirection = ParameterDirection.Input)
        {
            switch (argProviderName.ToLower())
            {
                case "system.data.sqlclient":
                default:
                    return CreateSqlParameter(argName, argSize, argValue, argDbType, argDirection);

            }
        }

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
