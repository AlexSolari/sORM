using sORM.Core.Mappings.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings
{
    internal class MapBinder
    {
        private Dictionary<Type, string> TypeMapping = new Dictionary<Type, string>()
        {
            [typeof(string)] = "VARCHAR(MAX)",
            [typeof(int)] = "int",
            [typeof(bool)] = "bit",
            [typeof(float)] = "real",
            [typeof(Guid)] = "uniqueidentifier"
        };


        public void Map(Type type)
        { 
            var props = type.GetProperties();

            var result = new Map();
            result.Name = type.Name;


            foreach (var prop in props)
            {
                string sqltype = string.Empty;
                foreach (Attribute attrib in prop.GetCustomAttributes(true))
                {
                    if (attrib.GetType() == typeof(KeyAttribute))
                    {
                        if (result.PrimaryKeyName != null)
                            throw new KeyAlreadyMappedException(type);

                        result.PrimaryKeyName = ((KeyAttribute)attrib).PropertyName;
                    }

                    if (attrib.GetType() == typeof(SecondaryKeyAttribute))
                    {
                        result.SecondaryKeyNames.Add(((SecondaryKeyAttribute)attrib).PropertyName);
                    }

                    if (attrib.GetType() == typeof(ReferenceToAttribute))
                    {
                        var attr = ((ReferenceToAttribute)attrib);
                        result.References.Add(attr.TypeToReference, new KeyValuePair<string, string>(attr.PropertyName, attr.CurrentPropertyName));
                    }

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

            if (string.IsNullOrWhiteSpace(result.PrimaryKeyName))
            {
                throw new KeyPropertyNotFoundException(type);
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
        {
            Map(typeof(TType));
        }
    }
}
