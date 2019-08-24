using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Requests.Concrete
{
    internal class AttachTriggerRequest<TType> : IRequest
    {
        public enum When
        {
            For,
            After,
            Instead
        }

        public enum RequestType
        {
            Insert,
            Update,
            Delete
        }

        public string SqlData { get; set; }

        public AttachTriggerRequest(string sql, When when, RequestType type)
        {
            var map = SimpleORM.Current.Mappings[typeof(TType)];
            var name = Guid.NewGuid().ToString().Replace("-", "");
            var whenExecute = "";
            switch (when)
            {
                case When.For:
                    whenExecute = "FOR";
                    break;
                case When.After:
                    whenExecute = "AFTER";
                    break;
                case When.Instead:
                    whenExecute = "INSTEAD OF";
                    break;
                default:
                    break;
            }
            var requestType = type.ToString().ToUpper();
            var tablename = map.Name;

            SqlData =
                "CREATE TRIGGER {0}" +
                "{1} {2} ON {3}" +
                "BEGIN" +
                sql +
                "END;";

            SqlData = string.Format(SqlData, name, whenExecute, requestType, tablename);
        }

        public IDbCommand BuildSql(SqlConnection connection)
        {
            return new SqlCommand(SqlData, connection);
        }
    }
}
