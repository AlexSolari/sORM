using sORM.Core.Conditions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data;

namespace sORM.Core.Requests.Concrete
{
    internal class SqlRequest : IRequest
    {
        public string Sql;

        public SqlRequest(string sql)
        {
            Sql = sql;
        }

        public IDbCommand BuildSql(SqlConnection connection)
        {
            return new SqlCommand(Sql, connection);
        }
    }
}
