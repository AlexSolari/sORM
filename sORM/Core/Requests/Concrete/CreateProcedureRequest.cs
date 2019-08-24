using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Requests.Concrete
{
    internal class CreateProcedureRequest : IRequest
    {
        public string SqlData { get; set; }

        public CreateProcedureRequest(string sql, string name)
        {
            SqlData = 
                "CREATE PROCEDURE " + name +
                @" AS
                BEGIN " +
                sql +
                " END;";
        }

        public IDbCommand BuildSql(SqlConnection connection)
        {
            return new SqlCommand(SqlData, connection);
        }
    }
}