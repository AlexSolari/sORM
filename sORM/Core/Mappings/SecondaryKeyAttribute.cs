using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings
{
    /// <summary>
    /// Marks decorated property to be a secondary key for entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class SecondaryKeyAttribute : Attribute
    {
        public string PropertyName { get; set; }

        public SecondaryKeyAttribute([CallerMemberName] string propertyName = null)
        {
            PropertyName = propertyName;
        }
    }
}
