using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class KeyAttribute : Attribute
    {
        public string PropertyName { get; set; }

        public KeyAttribute([CallerMemberName] string propertyName = null)
        {
            PropertyName = propertyName;
        }
    }
}
