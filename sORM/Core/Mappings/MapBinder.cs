using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings
{
    public class MapBinder
    {
        private Dictionary<Type, string> TypeMapping = new Dictionary<Type, string>()
        {
            [typeof(string)] = "varchar(MAX)",
            [typeof(int)] = "int",
            [typeof(bool)] = "bit",
            [typeof(float)] = "real",
        };


        public void Map(Type type)
        {
            if (!typeof(DataEntity).IsAssignableFrom(type))
            {
                throw new ArgumentException("Type to map into database must be inherited from sORM.Core.DataEntity");
            }

            var props = type.GetProperties();

            var result = new Map();
            result.Name = type.Name;


            foreach (var prop in props)
            {
                if (prop.Name.Equals("DataId"))
                {
                    result.Data.Add(prop, "DataId VARCHAR(36)");
                    continue;
                }

                string sqltype = string.Empty;
                foreach (object attrib in prop.GetCustomAttributes(true))
                {
                    if (attrib.GetType() == typeof(MapAsTypeAttribute))
                    {
                        sqltype = ((MapAsTypeAttribute)attrib).Type;
                    }
                    else if (attrib.GetType() == typeof(MapAutoAttribute))
                    {
                        sqltype = DetectType(prop.PropertyType);
                    }
                }

                if (string.IsNullOrWhiteSpace(sqltype))
                    continue;

                var parsedDefinition = prop.Name + " " + sqltype;
                result.Data.Add(prop, parsedDefinition);
            }

            SimpleORM.Current.Mappings.Add(type, result);
        }

        private string DetectType(Type propertyType, bool useStringAsDefault = true)
        {
            string result;
            if (TypeMapping.ContainsKey(propertyType))
            {
                result = TypeMapping[propertyType];
            }
            else if (useStringAsDefault)
            {
                result = TypeMapping[typeof(string)];
            }
            else
            {
                throw new ArgumentException("Can't map .NET type to SQL type.");
            }
            return result;
        }

        public void Map<TType>()
            where TType: DataEntity
        {
            Map(typeof(TType));
        }
    }
}
