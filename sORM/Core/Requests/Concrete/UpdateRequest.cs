using sORM.Core.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var keys = new List<string>();
            var values = new List<string>();
            foreach (var item in map.Data)
            {
                var value = item.Key.GetValue(Target);
                keys.Add(item.Key.Name);
                values.Add((value is String || value is Guid) ? string.Format("'{0}'", value) : value.ToString());
            }

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

            return "UPDATE " + map.Name + " SET " + string.Join(",", prms) + cnds;
        }

        public void AddParameter(string fieldname, object value)
        {
            parameters.Add(fieldname, (value is String || value is Guid) ? string.Format("'{0}'", value) : value.ToString());
        }

        public void AddCondition(ICondition condition)
        {
            Conditions.Add(condition);
        }
    }
}

