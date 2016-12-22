using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Conditions
{
    public interface ICondition
    {
        Dictionary<string, object> Parameters { get; }
        string BuildSql();
    }
}
