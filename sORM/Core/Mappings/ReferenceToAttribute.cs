using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings
{
    /// <summary>
    /// Creates a reference to another entity.
    /// Referenced field needs to be primary or secondary key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ReferenceToAttribute : Attribute
    {
        public Type TypeToReference { get; set; }

        public string PropertyName { get; set; }

        public string CurrentPropertyName { get; set; }

        public ReferenceToAttribute(Type TypeToReference, string PropertyName, [CallerMemberName] string currentPropName = null)
        {
            this.TypeToReference = TypeToReference;
            this.PropertyName = PropertyName;
            this.CurrentPropertyName = currentPropName;
        }
    }
}
