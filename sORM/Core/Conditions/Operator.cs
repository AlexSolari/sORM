using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Conditions
{
    public static class Operator
    {
        /// <summary>
        /// Joins two conditions under AND operator.
        /// </summary>
        /// <param name="first">First condition</param>
        /// <param name="second">Second condition</param>
        /// <returns>Created condition</returns>
        public static ICondition And(ICondition first, ICondition second)
        {
            return new RequestConditionGroup()
            {
                First = first,
                Second = second,
                Operator = " AND "
            };
        }

        /// <summary>
        /// Joins two conditions under OR operator.
        /// </summary>
        /// <param name="first">First condition</param>
        /// <param name="second">Second condition</param>
        /// <returns>Created condition</returns>
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
