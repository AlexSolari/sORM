using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Requests.Concrete
{
    public class CreateRequest : IRequest
    {
        private object Target;

        public CreateRequest(DataEntity objToCreate)
        {
            Target = objToCreate;
        }

        public string BuildSql()
        {
            var map = SimpleORM.Current.Mappings[Target.GetType()];
            var keys = new List<string>();
            var values = new List<string>();
            foreach (var item in map.Data)
            {
                var value = item.Key.GetValue(Target);
                keys.Add(item.Key.Name);
                values.Add((value is String || value is Guid) ? string.Format("'{0}'", value) : value.ToString());
            }
            return "INSERT INTO " + map.Name + " (" + string.Join(",", keys) + ")" + " VALUES " + " (" + string.Join(",", values) + ")";
        }
    }
}
