using sORM.Core.Conditions;
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
    public class DeleteRequest : IConditionalRequest
    {
        public IList<ICondition> Conditions { get; set; }
        private string tableName;
        private Type TargetType;

        public DeleteRequest(object obj)
        {
            TargetType = obj.GetType();
            tableName = obj.GetType().Name;
            Conditions = new List<ICondition>();

            var map = SimpleORM.Current.Mappings[TargetType];
            AddCondition(Condition.Equals(map.PrimaryKeyName, TargetType.GetProperty(map.PrimaryKeyName).GetValue(obj)));
        }

        public DeleteRequest(Type type)
        {
            Conditions = new List<ICondition>();
            tableName = type.Name;
            TargetType = type;
        }

        public void AddCondition(ICondition condition)
        {
            Conditions.Add(condition);
        }

        public IDbCommand BuildSql()
        {
            var map = SimpleORM.Current.Mappings[TargetType];

            var request = "DELETE FROM [" + tableName + "] ";

            if (Conditions.Count > 0) 
            { 
                var last = Conditions.Last();

                request += " WHERE ";
                foreach (var item in Conditions)
                {
                    request += item.BuildSql();

                    if (!last.Equals(item))
                        request += " AND ";
                }
            }

            var command = new SqlCommand(request, SimpleORM.Current.Requests.connection.Connection as SqlConnection);

            foreach (RequestCondition item in Conditions)
            {
                foreach (var parameter in item.Parameters)
                {
                    object value;
                    if (parameter.Value == null)
                    {
                        value = DBNull.Value;
                    }
                    else if (parameter.Value is string || parameter.Value is Guid || parameter.Value is DateTime)
                    {
                        value = parameter.Value.ToString();
                    }
                    else if (parameter.Value is bool)
                    {
                        value = parameter.Value;
                    }
                    else if (parameter.Value is XmlDocument)
                    {
                        value = ((XmlDocument)parameter.Value).InnerXml;
                    }
                    else
                    {
                        value = parameter.Value.ToString();
                    }
                    command.Parameters.Add(parameter.Key, map.GetSqlType(item.Field)).Value = value;
                }
            }

            return command;
        }
    }
}
