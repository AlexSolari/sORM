using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Requests
{
    public interface IRequest
    {
        IDbCommand BuildSql();
    }
}
