using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings
{
    public class Map
    {
        public Dictionary<PropertyInfo, string> Data = new Dictionary<PropertyInfo, string>();
        public string Name;
        public string PrimaryKeyName;
        public List<string> SecondaryKeyNames;
        public Dictionary<Type, KeyValuePair<string, string>> References;

        public Map()
        {
            References = new Dictionary<Type, KeyValuePair<string, string>>();
            SecondaryKeyNames = new List<string>();
            PrimaryKeyName = null;
        }

        public System.Data.SqlDbType GetSqlType(string key)
        {
            var info = Data.Keys.SingleOrDefault(x => x.Name.Equals(key));

            if (info != null)
            {
                if (info.PropertyType == typeof(string))
                {
                    return System.Data.SqlDbType.VarChar;
                }
                else if (info.PropertyType == typeof(int))
                {
                    return System.Data.SqlDbType.Int;
                }
                else if (info.PropertyType == typeof(float))
                {
                    return System.Data.SqlDbType.Real;
                }
                else if (info.PropertyType == typeof(bool))
                {
                    return System.Data.SqlDbType.Bit;
                }
                else if (info.PropertyType == typeof(Guid))
                {
                    return System.Data.SqlDbType.UniqueIdentifier;
                }
            }
            return System.Data.SqlDbType.VarChar;
        }
    }
}
