using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Conditions
{
    public static class Condition
    {
        public static ICondition Equals(string field, object value)
        {
            return new RequestCondition()
            {
                Field = field,
                Operator = "=",
                Value = value
            };
        }

        public static ICondition NotEquals(string field, object value)
        {
            return new RequestCondition()
            {
                Field = field,
                Operator = "!=",
                Value = value
            };
        }

        public static ICondition More(string field, object value, bool strict = false)
        {
            return new RequestCondition()
            {
                Field = field,
                Operator = (strict) ? ">=" : ">",
                Value = value
            };
        }

        public static ICondition Less(string field, object value, bool strict = false)
        {
            return new RequestCondition()
            {
                Field = field,
                Operator = (strict) ? "<=" : "<",
                Value = value
            };
        }

        public static ICondition Like(string field, string value)
        {
            return new RequestCondition()
            {
                Field = field,
                Operator = " LIKE ",
                Value = value
            };
        }
    }
}
