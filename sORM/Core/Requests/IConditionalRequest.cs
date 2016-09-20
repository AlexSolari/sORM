using sORM.Core.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Requests
{
    public interface IConditionalRequest : IRequest
    {
        IList<ICondition> Conditions { get; set; }
        void AddCondition(ICondition condition);
    }
}
