using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Conditions
{
    public static class Condition
    {
        /// <summary>
        /// Creates a condition that will check if field equals value.
        /// </summary>
        /// <param name="field">Field name to check.</param>
        /// <param name="value">Value to compare.</param>
        /// <returns>Created condition</returns>
        public static ICondition Equals(string field, object value)
        {
            return new RequestCondition()
            {
                Field = field,
                Operator = "=",
                Value = value
            };
        }

        /// <summary>
        /// Creates a condition that will check if field not equals value.
        /// </summary>
        /// <param name="field">Field name to check.</param>
        /// <param name="value">Value to compare.</param>
        /// <returns>Created condition</returns>
        public static ICondition NotEquals(string field, object value)
        {
            return new RequestCondition()
            {
                Field = field,
                Operator = "!=",
                Value = value
            };
        }

        /// <summary>
        /// Creates a condition that will check if field greater than value.
        /// </summary>
        /// <param name="field">Field name to check.</param>
        /// <param name="value">Value to compare.</param>
        /// <param name="strict">If false is passed, will check if greater or equals</param>
        /// <returns>Created condition</returns>
        public static ICondition More(string field, object value, bool strict = false)
        {
            return new RequestCondition()
            {
                Field = field,
                Operator = (strict) ? ">=" : ">",
                Value = value
            };
        }

        /// <summary>
        /// Creates a condition that will check if field lesser than value.
        /// </summary>
        /// <param name="field">Field name to check.</param>
        /// <param name="value">Value to compare.</param>
        /// <param name="strict">If false is passed, will check if lesser or equals</param>
        /// <returns>Created condition</returns>
        public static ICondition Less(string field, object value, bool strict = false)
        {
            return new RequestCondition()
            {
                Field = field,
                Operator = (strict) ? "<=" : "<",
                Value = value
            };
        }

        /// <summary>
        /// Creates a condition that will check if field is like value.
        /// </summary>
        /// <param name="field">Field name to check.</param>
        /// <param name="value">Value to compate. Use Transact-SQL LIKE operator syntax</param>
        /// <returns>Condition</returns>
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
