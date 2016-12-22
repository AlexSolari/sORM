using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Conditions
{
    public class RequestCondition : RequestConditionBase, ICondition
    {
        public string Field { get; set; }
        public string Operator { get; set; }

        private object _value;
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                ParameterValue = value;
                _value = value;
            }
        }

        public override Dictionary<string, object> Parameters
        {
            get
            {
                return new Dictionary<string, object>() { [ParameterName] = ParameterValue };
            }
        }

        public override string BuildSql()
        {
            return Field + Operator + ParameterName;
        }
    }
}
