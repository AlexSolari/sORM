using sORM.Core.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sORM.Core.Requests.Concrete
{
    public class UpdateRequest : IParametrizedRequest, IConditionalRequest
    {
        public IList<ICondition> Conditions { get; set; }
        private Dictionary<string, string> parameters = new Dictionary<string, string>();
        private object Target;

        public UpdateRequest(DataEntity objToUpdate = null)
        {
            Conditions = new List<ICondition>();
            Target = objToUpdate;
            if (objToUpdate != null)
            {
                AddCondition(Condition.Equals("DataId", objToUpdate.DataId));
                var props = objToUpdate.GetType().GetProperties();
                foreach (var p in props)
                {
                    AddParameter(p.Name, p.GetValue(objToUpdate));
                }
            }
        }

        public string BuildSql()
        {
            var map = SimpleORM.Current.Mappings[Target.GetType()];

            var prms = parameters.Select(x => x.Key + "=" + x.Value);

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

            return "UPDATE [" + map.Name + "] SET " + string.Join(",", prms) + cnds;
        }

        public void AddParameter(string fieldname, object value)
        {
            string res;
            if (value == null)
            {
                res = "NULL";
            }
            else if (value is string || value is Guid || value is DateTime)
            {
                res = string.Format("'{0}'", value);
            }
            else if (value is bool)
            {
                res = ((bool)value) ? "1" : "0";
            }
            else if (value is XmlDocument)
            {
                res = "'" + ((XmlDocument)value).InnerXml + "'";
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

