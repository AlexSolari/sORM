using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sORM.Core.Mappings.Exceptions
{
    internal class KeyAlreadyMappedException : Exception
    {
        public KeyAlreadyMappedException(Type type) : base("Key property is already mapped for class " + type.FullName)
        {

        }
    }
}
