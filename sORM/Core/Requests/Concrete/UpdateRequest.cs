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
    public class UpdateRequest : IParametrizedRequest, IConditionalRequest
    {
        public IList<ICondition> Conditions { get; set; }
        private Dictionary<string, object> parameters = new Dictionary<string, object>();
        private object Target;
        private Type TargetType;

        public UpdateRequest(object objToUpdate = null)
        {
            Conditions = new List<ICondition>();
            Target = objToUpdate;
            TargetType = objToUpdate.GetType();
            if (objToUpdate != null)
            {
                var map = SimpleORM.Current.Mappings[TargetType];
                AddCondition(Condition.Equals(map.KeyName, TargetType.GetProperty(map.KeyName).GetValue(objToUpdate)));

                var props = objToUpdate.GetType().GetProperties();
                foreach (var p in props)
                {
                    AddParameter(p.Name, p.GetValue(objToUpdate));
                }
            }
        }

        public IDbCommand BuildSql()
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
            var command = new SqlCommand(commandText, SimpleORM.Current.Requests.connection.Connection as SqlConnection);

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
                res = "NULL";
            }
            /*else if (value is string || value is Guid || value is DateTime)
            {
                res = string.Format("{0}", value);
            }*/
            else if (value is bool)
            {
                res = value.ToString();
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

