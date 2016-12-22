using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Conditions
{
    public abstract class RequestConditionBase : ICondition
    {
        public string ParameterName { get; set; } = "@"+Guid.NewGuid().ToString().Replace("-","");
        public abstract Dictionary<string, object> Parameters { get; }
        public object ParameterValue { get; set; }

        public abstract string BuildSql();
    }
}
