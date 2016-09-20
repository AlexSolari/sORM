using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Conditions
{
    public class RequestCondition : ICondition
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }

        public string BuildSql()
        {
            return Field + Operator + Value;
        }
    }
}
