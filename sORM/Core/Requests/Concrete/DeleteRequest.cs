using sORM.Core.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Requests.Concrete
{
    public class DeleteRequest : IConditionalRequest
    {
        public IList<ICondition> Conditions { get; set; }
        private string tableName;

        public DeleteRequest(DataEntity obj)
        {
            tableName = obj.GetType().Name;
            Conditions = new List<ICondition>();
            AddCondition(Condition.Equals("DataId", obj.DataId));
        }

        public DeleteRequest(Type type)
        {
            Conditions = new List<ICondition>();
            tableName = type.Name;
        }

        public void AddCondition(ICondition condition)
        {
            Conditions.Add(condition);
        }

        public string BuildSql()
        {
            var request = "DELETE FROM [" + tableName + "] ";

            if (Conditions.Count > 0) 
            { 
                var last = Conditions.Last();

                request += " WHERE ";
                foreach (var item in Conditions)
                {
                    request += item.BuildSql();

                    if (!last.Equals(item))
                        request += " AND ";
                }
            }
            return request;
        }
    }
}
