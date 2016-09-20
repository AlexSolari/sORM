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
    }
}
