using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
                if (value == null)
                {
                    values.Add("NULL");
                }
                else if (value is string || value is Guid || value is DateTime)
                {
                    values.Add(string.Format("'{0}'", value));
                }
                else if (value is bool)
                {
                    values.Add(((bool)value) ? "1" : "0");
                }
                else if (value is XmlDocument)
                {
                    values.Add("'" + ((XmlDocument)value).InnerXml + "'");
                }
                else
                {
                    values.Add(value.ToString());
                }
            }
            return "INSERT INTO [" + map.Name + "] (" + string.Join(",", keys) + ")" + " VALUES " + " (" + string.Join(",", values) + ")";
        }
    }
}
