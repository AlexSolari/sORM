using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Conditions
{
    public static class Operator
    {
        public static ICondition And(ICondition first, ICondition second)
        {
            return new RequestConditionGroup()
            {
                First = first,
                Second = second,
                Operator = " AND "
            };
        }

        public static ICondition Or(ICondition first, ICondition second)
        {
            return new RequestConditionGroup()
            {
                First = first,
                Second = second,
                Operator = " OR "
            };
        }
    }
}
