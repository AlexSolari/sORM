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
    internal class UpdateRequest : IParametrizedRequest, IConditionalRequest
    {
        public IList<ICondition> Conditions { get; set; }
        private Dictionary<string, object> parameters = new Dictionary<string, object>();
        private object Target;
        private Type TargetType;

        public UpdateRequest(object objToUpdate = null)
        {
            Conditions = new List<ICondition>();
            Target = objToUpdate;
            if (objToUpdate != null)
            {
                TargetType = objToUpdate.GetType();

                var map = SimpleORM.Current.Mappings[TargetType];
                AddCondition(Condition.Equals(map.PrimaryKeyName, TargetType.GetProperty(map.PrimaryKeyName).GetValue(objToUpdate)));

                foreach (var p in map.Data.Keys)
                {
                    AddParameter(p.Name, p.GetValue(objToUpdate));
                }
            }
        }

        public IDbCommand BuildSql(SqlConnection connection)
        {
            var map = SimpleORM.Current.Mappings[Target.GetType()];

            var generatedPrms = parameters.ToDictionary(y => y.Key, x => "@SqlParam" + x.Key);
            var prms = parameters.Select(x => x.Key + "=" + generatedPrms[x.Key]);

            var cnds = string.Empty;
            if (Conditions.Count > 0)
            {
                var last = Conditions.Last();

                cnds += " WHERE ";
                foreach (var item in Conditions)
                {
                    cnds += item.BuildSql();

                    if (!last.Equals(item))
                        cnds += " AND ";
                }
            }
            
            var commandText = "UPDATE [" + map.Name + "] SET " + string.Join(",", prms) + cnds;
            var command = new SqlCommand(commandText, connection);

            foreach (var item in generatedPrms)
            {
                command.Parameters.Add(item.Value, map.GetSqlType(item.Key)).Value = parameters[item.Key];
            }

            foreach (RequestCondition item in Conditions)
            {
                foreach (var parameter in item.Parameters)
                {
                    command.Parameters.Add(parameter.Key, map.GetSqlType(item.Field)).Value = parameter.Value;
                }
            }

            return command;
        }

        public void AddParameter(string fieldname, object value)
        {
            object res;
            if (value == null)
            {
                res = DBNull.Value;
            }
            else if (value is XmlDocument)
            {
                res = ((XmlDocument)value).InnerXml;
            }
            else if (value is Guid)
            {
                res = value;
            }
            else
            {
                res = value.ToString();
            }
            parameters.Add(fieldname, res);
        }

        public void AddCondition(ICondition condition)
        {
            Conditions.Add(condition);
        }
    }
}

