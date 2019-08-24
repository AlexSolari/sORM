using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Conditions
{
    internal class RequestConditionGroup : ICondition
    {
        public ICondition First { get; set; }
        public ICondition Second { get; set; }
        public string Operator { get; set; }

        public Dictionary<string, object> Parameters
        {
            get
            {
                return First.Parameters.Concat(Second.Parameters).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public string BuildSql()
        {
            return string.Format("({0}) {1} ({2})", First.BuildSql(), Operator, Second.BuildSql());
        }
    }
}
