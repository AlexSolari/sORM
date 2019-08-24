using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sORM.Core.Requests.Concrete
{
    internal class CreateRequest : IRequest
    {
        private object Target;

        public CreateRequest(object objToCreate)
        {
            Target = objToCreate;
        }

        public IDbCommand BuildSql(SqlConnection connection)
        {
            var map = SimpleORM.Current.Mappings[Target.GetType()];
            var keys = new List<string>();
            var values = new List<object>();
            foreach (var item in map.Data)
            {
                var value = item.Key.GetValue(Target);
                keys.Add(item.Key.Name);
                if (value == null)
                {
                    values.Add(DBNull.Value);
                }
                else if (value is bool || value is Guid)
                {
                    values.Add(value);
                }
                else if (value is XmlDocument)
                {
                    values.Add(((XmlDocument)value).InnerXml);
                }
                else
                {
                    values.Add(value.ToString());
                }
            }

            var generatedKeys = keys.Select(x => "@SqlParam" + x).ToList();
            var commandText = "INSERT INTO [" + map.Name + "] (" + string.Join(",", keys) + ")" + " VALUES " + " (" + string.Join(",", generatedKeys) + ")";
            var command = new SqlCommand(commandText, connection);
            
            foreach (var item in generatedKeys.ToDictionary(x => x, y => values[generatedKeys.IndexOf(y)]))
            {
                command.Parameters.Add(item.Key, map.GetSqlType(item.Key.Replace("@SqlParam", ""))).Value = item.Value;
            }

            return command;
        }
    }
}
