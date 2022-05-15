using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRAnalytics.DAL
{
    public interface IDatabase
    {
        IDbConnection CreateConnection();

        void CloseConnection(IDbConnection argConnection);

        IDbCommand CreateCommand(string argCommandText, CommandType argCommandType, IDbConnection argConnection);

        IDataAdapter CreateAdapter(IDbCommand argCommand);

        IDbDataParameter CreateParameter(IDbCommand argcommand);
    }
}
