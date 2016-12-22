using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited = true)]
    public class MapAutoAttribute : Attribute
    {
        public MapAutoAttribute() { }
    }
}
