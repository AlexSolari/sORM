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
    public class SqlRequest : IRequest
    {
        public string Sql;

        public SqlRequest(string sql)
        {
            Sql = sql;
        }

        public IDbCommand BuildSql()
        {
            return new SqlCommand(Sql, SimpleORM.Current.Requests.connection.Connection as SqlConnection);
        }
    }
}
